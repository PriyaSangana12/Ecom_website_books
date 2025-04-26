# BookCart Project Development Guide

## 1. Project Setup
### 1.1 Create ASP.NET Core with Angular Project
```bash
dotnet new angular -n BookCart -o BookCart
```
- Creates new project with Angular frontend
- Sets up basic project structure
- Configures ASP.NET Core Web API
- Configures Angular CLI

### 1.2 Key Initial Files
- **Program.cs**: 
  - Configures services and middleware pipeline
  - Sets up dependency injection
  - Configures routing and endpoints
- **appsettings.json**:
  - Stores connection strings
  - Configures JWT settings
  - Contains application configuration
- **ClientApp/**:
  - Contains Angular application
  - Includes components, services, routing
  - Uses Angular Material for UI

## 2. Database Layer
### 2.1 BookDBContext.cs
```csharp
public class BookDBContext : DbContext
{
    public BookDBContext(DbContextOptions<BookDBContext> options) : base(options) { }
    
    public DbSet<Book> Book { get; set; }
    public DbSet<UserMaster> UserMaster { get; set; }
    // Other DbSets...
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entity relationships
        modelBuilder.Entity<Book>()
            .HasKey(b => b.BookId);
        // Other configurations...
    }
}
```
- Manages database connections
- Configures entity relationships
- Handles database migrations

### 2.2 Book.cs (Entity Model)
```csharp
public class Book
{
    [Key]
    public int BookId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Title { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Author { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    // Other properties with data annotations...
}
```
- Represents database table structure
- Uses data annotations for validation
- Maps to database columns

## 3. Data Access Layer
### 3.1 BookDataAccessLayer.cs
```csharp
public class BookDataAccessLayer : IBookService
{
    private readonly BookDBContext _db;
    
    public BookDataAccessLayer(BookDBContext db)
    {
        _db = db;
    }

    public IEnumerable<Book> GetAllBooks()
    {
        return _db.Book
            .OrderBy(b => b.Title)
            .ToList();
    }

    public Book GetBookData(int id)
    {
        return _db.Book
            .FirstOrDefault(b => b.BookId == id);
    }
    
    // Other CRUD operations with error handling...
}
```
- Implements repository pattern
- Handles database operations
- Includes query optimization
- Manages transactions

## 4. Business Logic Layer
### 4.1 Interfaces (IBookService.cs)
```csharp
public interface IBookService
{
    IEnumerable<Book> GetAllBooks();
    Book GetBookData(int id);
    int AddBook(Book book);
    int UpdateBook(Book book);
    int DeleteBook(int id);
    // Other business operations...
}
```
- Defines service contracts
- Enables dependency injection
- Supports unit testing
- Promotes loose coupling

## 5. API Controllers
### 5.1 BookController.cs
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    
    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Book>> Get()
    {
        var books = _bookService.GetAllBooks();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public ActionResult<Book> Get(int id)
    {
        var book = _bookService.GetBookData(id);
        if (book == null) return NotFound();
        return Ok(book);
    }
    
    // Other CRUD endpoints with proper status codes...
}
```
- Handles HTTP requests
- Implements RESTful conventions
- Uses dependency injection
- Includes proper status codes
- Handles exceptions

## 6. Authentication
### 6.1 LoginController.cs
```csharp
[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;
    
    public LoginController(IUserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }

    [HttpPost]
    public IActionResult Post([FromBody] UserLogin login)
    {
        var user = _userService.AuthenticateUser(login);
        if (user == null) return Unauthorized();
        
        var token = GenerateJWTToken(user);
        return Ok(new { token });
    }
    
    private string GenerateJWTToken(UserMaster user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            
        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);
            
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("role", user.UserType.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);
            
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```
- Implements JWT authentication
- Handles user login
- Generates secure tokens
- Manages token expiration
- Includes role-based authorization

## 7. Frontend (Angular)
### 7.1 Book Service
```typescript
@Injectable({
  providedIn: 'root'
})
export class BookService {
  private apiUrl = `${environment.apiUrl}/api/book`;
  
  constructor(private http: HttpClient) {}

    getBooks(): Observable<Book[]> {
        return this.http.get<Book[]>(this.apiUrl)
            .pipe(
                catchError(this.handleError)
            );
    }
    
    private handleError(error: HttpErrorResponse) {
        // Error handling logic...
    }
}
```
- Makes HTTP requests to backend
- Handles errors
- Uses RxJS observables
- Implements retry logic

### 7.2 Book Card Component
```typescript
@Component({
    selector: 'app-book-card',
    templateUrl: './book-card.component.html',
    styleUrls: ['./book-card.component.scss']
})
export class BookCardComponent implements OnInit {
    @Input() book: Book;
    
    constructor(private cartService: CartService,
                private router: Router) {}

    ngOnInit(): void {
        // Component initialization
    }

    addToCart(): void {
        this.cartService.addToCart(this.book)
            .subscribe(
                () => this.showSuccess(),
                error => this.showError(error)
            );
    }
    
    private showSuccess(): void {
        // Show success message
    }
    
    private showError(error: any): void {
        // Show error message
    }
}
```
- Displays book information
- Handles user interactions
- Manages component lifecycle
- Implements error handling

## 8. Database Initialization
### 8.1 BookSeeder.cs
```csharp
public static class BookSeeder
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new BookDBContext(
            serviceProvider.GetRequiredService<DbContextOptions<BookDBContext>>());

        if (context.Book.Any()) return;

        var books = new List<Book>
        {
            new Book { Title = "Clean Code", Author = "Robert Martin", Price = 45.99m },
            new Book { Title = "Design Patterns", Author = "Erich Gamma", Price = 54.99m },
            // More books...
        };

        context.Book.AddRange(books);
        context.SaveChanges();
    }
}
```
- Seeds initial data
- Checks for existing data
- Uses bulk operations
- Handles database transactions

## 9. Running the Project
### 9.1 Development
```bash
cd BookCart
dotnet run
```
- Starts development server
- Enables hot reload
- Configures development environment
- Uses development database

### 9.2 Production
```bash
dotnet publish -c Release -o ./publish
```
- Creates optimized build
- Bundles Angular assets
- Configures production settings
- Prepares for deployment

## 10. Testing
1. **Unit Testing**:
    - Test controllers
    - Test services
    - Test components
2. **Integration Testing**:
    - Test API endpoints
    - Test database operations
3. **End-to-End Testing**:
    - Test user flows
    - Test UI interactions

## 11. Deployment
1. **Database Setup**:
    - Create production database
    - Apply migrations
    - Seed initial data
2. **Web Server Configuration**:
    - Set up IIS/Nginx
    - Configure reverse proxy
    - Set up SSL
3. **Application Configuration**:
    - Set environment variables
    - Configure logging
    - Set up monitoring

## Troubleshooting
1. **Database Issues**:
    - Check connection strings
    - Verify database permissions
    - Check migration status
2. **Authentication Problems**:
    - Verify JWT settings
    - Check token expiration
    - Validate user roles
3. **Performance Issues**:
    - Optimize database queries
    - Implement caching
    - Monitor resource usage
