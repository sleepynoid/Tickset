# NexaDesk Learning Notes

Catatan pembelajaran C# dan .NET untuk developer dari ekosistem JavaScript/TypeScript, NestJS, dan Laravel.

---

## 1. Entity dan BaseModel

### Konsep

Entity adalah class yang merepresentasikan tabel di database. BaseEntity adalah class abstract yang menjadi "cetakan" untuk semua entity lain.

### Perbandingan

| .NET (EF Core) | Prisma | TypeORM | NestJS |
|----------------|--------|---------|--------|
| `abstract class BaseEntity` | `model BaseEntity` | `@Entity()` base class | N/A |
| `class User : BaseEntity` | `model User` | `@Entity()` class | N/A |
| `public Guid Id { get; set; }` | `id String @id @default(uuid())` | `@PrimaryGeneratedColumn('uuid')` | N/A |
| `public DateTime CreatedAt` | `createdAt DateTime @default(now())` | `@CreateDateColumn()` | N/A |
| `public DateTime? UpdatedAt` | `updatedAt DateTime @updatedAt` | `@UpdateDateColumn()` | N/A |

### Contoh C#

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
}
```

### Penjelasan

- `abstract class` → class yang tidak bisa diinstansiasi langsung, hanya diwarisi. Mirip abstract class di TypeScript.
- `Guid` → tipe ID unik seperti `uuid` di PostgreSQL atau `string` type di Prisma.
- `DateTime` → tipe waktu seperti `Date` atau `DateTime` di TypeScript.
- `DateTime?` → nullable, bisa null. Mirip `Date | null` di TypeScript.
- `= DateTime.UtcNow` → default value saat object dibuat. Mirip `@default(now())` di Prisma.
- `= string.Empty` → default value string kosong, menghindari null reference warning.

### Kapan pakai inheritance vs DI

| Gunakan Inheritance | Gunakan DI |
|---------------------|------------|
| Saat ingin share field/method ke banyak class | Saat class membutuhkan service lain |
| Contoh: `BaseEntity` untuk ID dan timestamps | Contoh: `UserService` butuh `DbContext` |

---

## 2. Namespace

### Konsep

Namespace adalah "alamat" unik untuk setiap class di .NET. Mirip `package` di Java atau `module` di TypeScript.

### Perbandingan

| .NET | TypeScript | NestJS |
|------|------------|--------|
| `namespace Tickset.Models` | `export namespace Models` | `@Module()` |
| `using Tickset.Models` | `import { Models } from './models'` | `import` di module |

### Contoh C#

```csharp
namespace Tickset.Models;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
}
```

### Penjelasan

- `namespace Tickset.Models` → group class `User` ke dalam namespace `Tickset.Models`.
- File yang berbeda bisa akses `User` dengan `using Tickset.Models;`.
- Konvensi: namespace mengikuti folder structure.
- Tanpa namespace, compiler tidak bisa menemukan class lain yang reference.

### Kenapa wajib ada

```csharp
// Tanpa namespace → ERROR
public class User : BaseEntity
{
    // Error: The type or namespace name 'BaseEntity' does not exist
}

// Dengan namespace → BERHASIL
namespace Tickset.Models;

public class User : BaseEntity
{
    // BaseEntity ditemukan karena satu namespace
}
```

---

## 3. Dependency Injection (DI)

### Konsep

DI adalah cara menyediakan "kebutuhan" (dependency) ke sebuah class, tanpa class itu harus membuatnya sendiri.

### Perbandingan

| .NET | NestJS | Laravel |
|------|--------|---------|
| `builder.Services.AddDbContext<T>()` | `@Module({ providers: [Service] })` | Service Provider |
| `constructor(T context)` | `constructor(private prisma: PrismaService)` | Constructor injection |
| `AddScoped<T>()` | Default scope | Singleton (default) |
| `AddSingleton<T>()` | `@Injectable({ scope: Scope.DEFAULT })` | `singleton()` |
| `AddTransient<T>()` | `@Injectable()` | `bind()` |

### Contoh C#

```csharp
// Daftarkan service di Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(...);
builder.Services.AddScoped<UserService>();

// Mintalah di constructor
public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }
}
```

### Penjelasan

- `builder.Services.AddDbContext<T>()` → mendaftarkan service ke DI Container (gudang penyimpanan).
- `AddScoped<T>()` → service baru dibuat per request, di-share dalam satu request.
- `AddSingleton<T>()` → satu instance untuk seluruh aplikasi.
- `AddTransient<T>()` → instance baru setiap kali diminta.

### 3 Jenis Lifetime

| Lifetime | Analogi | Kapan digunakan |
|----------|---------|-----------------|
| Transient | Kopi baru setiap kali dipesan | Service ringan, tidak berubah |
| Scoped | Kopi dibagi per meja/request | Database context |
| Singleton | Satu mesin kopi untuk semua | Cache, config |

### Tanpa DI vs Dengan DI

```csharp
// Tanpa DI — UserService harus buat sendiri
public class UserService
{
    private readonly ApplicationDbContext _context = new ApplicationDbContext();
    // Masalah: tidak bisa diganti, tidak bisa di-test
}

// Dengan DI — DI menyediakan
public class UserService
{
    private readonly ApplicationDbContext _context;
    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }
}
```

---

## 4. DbContext

### Konsep

ApplicationDbContext adalah jembatan antara kode C# dan database. Mirip `PrismaClient` di Prisma atau `Connection` di TypeORM.

### Perbandingan

| .NET (EF Core) | Prisma | TypeORM | Laravel (Eloquent) |
|----------------|--------|---------|-------------------|
| `ApplicationDbContext` | `PrismaClient` | `Connection` | Eloquent |
| `DbSet<User> Users` | `prisma.user` | `getRepository(User)` | `User::class` |
| `context.Users.AddAsync(user)` | `prisma.user.create({ data })` | `repository.save(user)` | `User::create($data)` |
| `context.Users.ToListAsync()` | `prisma.user.findMany()` | `repository.find()` | `User::all()` |
| `context.SaveChangesAsync()` | `prisma.$commit()` | `connection.commit()` | Otomatis |

### Contoh C#

```csharp
using Microsoft.EntityFrameworkCore;
using Tickset.Models;

namespace Tickset.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<TeamMember>()
            .HasKey(tm => new { tm.UserId, tm.TeamId });

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });
    }
}
```

### Penjelasan

- `DbSet<T>` → mendaftarkan tabel ke database. Mirip `model` di Prisma.
- `Set<T>()` → shorthand untuk `new DbSet<T>()`.
- `OnModelCreating` → method untuk konfigurasi model (key, relasi, constraint).
- `.HasKey()` → menentukan primary key untuk junction table.

### Kenapa di folder `Data/`

Folder `Data/` adalah konvensi di .NET untuk file yang berhubungan dengan akses database. Mirip folder `prisma/` di Node.js.

```
Tickset/
├── Data/
│   └── ApplicationDbContext.cs   ← pintu masuk ke database
├── Models/
│   ├── BaseEntity.cs            ← base class
│   ├── User.cs                  ← model User
│   └── ...
```

### Apakah DbContext global?

Ya, tapi bukan berarti data bisa diakses sembarang orang. DbContext didaftarkan ke DI, yang artinya tersedia di seluruh aplikasi, tetapi tetap ada kontrol aksesnya (authorization).

### Kenapa semua DbSet ditambah ke DbContext?

Karena DbContext adalah satu-satunya pintu masuk ke database. Semua tabel harus didaftarkan di sini agar EF Core tahu adanya tabel tersebut. Tanpa `DbSet<T>`, EF Core tidak tahu tabel itu ada.

---

## 5. Prisma vs EF Core

### Perbandingan Umum

| Aspek | Prisma | EF Core |
|-------|--------|---------|
| Definisi model | Schema file (.prisma) | Class C# (.cs) |
| Generate client | `prisma generate` | Tidak perlu |
| Registration | Import di code | Daftar manual di DI |
| Query | PrismaClient methods | LINQ + DbContext |
| Migration | `prisma migrate dev` | `dotnet ef migrations add` |
| Primary key | Otomatis (id) | Otomatis (Id) atau manual |
| Junction table | Tidak perlu model | **Wajib** model + key |

### Kenapa EF Core tidak perlu generate?

Karena EF Core menggunakan **reflection** — ia membaca class C# secara langsung saat runtime, bukan dari file schema terpisah.

```csharp
// Anda tulis class ini:
public class User : BaseEntity
{
    public string Email { get; set; }
}

// EF Core otomatis tahu:
// - Tabel "Users" perlu dibuat
// - Kolom "Email" bertipe varchar
// - Kolom "Id" adalah primary key (dari BaseEntity)
```

---

## 6. Many-to-Many Relationship

### Konsep

Junction table (pivot table) digunakan untuk relasi many-to-many antar tabel.

### Perbandingan

| Laravel | .NET EF Core | Prisma |
|---------|--------------|--------|
| Pivot table = migration saja | Pivot table = **model + DbSet** | Tidak perlu model |
| `belongsToMany()` | `ICollection<T>` navigation | `relations` |
| `attach()` / `detach()` | `AddAsync()` / `Remove()` | `connect()` / `disconnect()` |
| Tidak perlu define primary key | **Wajib** define composite key | Otomatis |

### Contoh C#

```csharp
// Junction table — TIDAK mewarisi BaseEntity
public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

// Navigation property di entity
public class User : BaseEntity
{
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

// Composite key di DbContext
modelBuilder.Entity<UserRole>()
    .HasKey(ur => new { ur.UserId, ur.RoleId });
```

### Penjelasan

- Junction table tidak mewarisi `BaseEntity` karena tidak butuh `Id`, `CreatedAt`, `UpdatedAt`.
- `null!` → memberi tahu compiler bahwa property akan diisi oleh EF Core.
- Composite key = gabungan dua foreign key sebagai primary key.
- `ICollection<T>` → navigation property untuk akses data relasi.

### Junction table dengan field tambahan

```csharp
public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    // Field tambahan
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string? AssignedBy { get; set; }
}
```

---

## 7. Primary Key

### Konsep

Primary key adalah kolom unik yang mengidentifikasi setiap baris di tabel.

### Kapan perlu set manual

| Model | Primary Key | Perlu Set Manual? |
|-------|-------------|-------------------|
| Mewarisi `BaseEntity` | `Id` | Tidak |
| Junction table | Composite key | **Ya** |
| Model tanpa `Id` | Tidak ada | **Ya** |

### Contoh

```csharp
// Otomatis — ada property Id dari BaseEntity
public class User : BaseEntity { }

// Manual — junction table butuh composite key
modelBuilder.Entity<UserRole>()
    .HasKey(ur => new { ur.UserId, ur.RoleId });
```

### Error jika tidak set

```
The entity type 'RolePermission' requires a primary key to be defined.
```

---

## 8. Middleware Pipeline

### Konsep

Middleware adalah komponen yang memproses request sebelum sampai ke endpoint. Mirip middleware di Express.js atau NestJS.

### Perbandingan

| .NET | Express.js | NestJS |
|------|------------|--------|
| `app.UseMiddleware<T>()` | `app.use(middleware)` | `@Module()` + middleware |
| `app.UseAuthentication()` | Passport.js | `@nestjs/passport` |
| `app.UseAuthorization()` | CORS, Auth | Guards |

### Contoh C#

```csharp
var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/users", ...);
```

### Penjelasan

- Middleware dijalankan **berurutan** dari atas ke bawah.
- Setiap request melewati semua middleware sebelum sampai ke endpoint.
- Urutan penting: `UseAuthentication()` harus sebelum `UseAuthorization()`.

---

## 9. Minimal API vs Controllers

### Konsep

ASP.NET Core mendukung dua cara membuat endpoint: Minimal API dan Controllers.

### Perbandingan

| Minimal API | Controllers | NestJS | Next.js |
|-------------|-------------|--------|---------|
| `app.MapGet(...)` | `[HttpGet]` method | `@Get()` decorator | `export async function GET()` |
| Tidak ada class | Class dengan attribute | Class dengan decorator | Function |
| Ringkas | Terstruktur | Terstruktur | Ringkas |

### Contoh C#

```csharp
// Minimal API
app.MapGet("/api/users", async (ApplicationDbContext context) =>
{
    return await context.Users.ToListAsync();
});

// Controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public async Task<List<User>> GetUsers(ApplicationDbContext context)
    {
        return await context.Users.ToListAsync();
    }
}
```

---

## 10. DTO (Data Transfer Object)

### Konsep

DTO adalah class yang digunakan untuk transfer data antara client dan server. Mirip DTO di NestJS atau interface di TypeScript.

### Perbandingan

| .NET | NestJS | TypeScript |
|------|--------|------------|
| `record CreateUserDto(...)` | `class CreateUserDto` | `interface CreateUserDto` |
| `app.MapPost("/api", (CreateUserDto dto) => ...)` | `@Body() dto: CreateUserDto` | `body: CreateUserDto` |

### Contoh C#

```csharp
namespace Tickset.DTOs;

public record CreateUserDto(
    string Email,
    string FirstName,
    string LastName,
    string Password
);
```

### Penjelasan

- `record` → tipe data immutable (tidak bisa diubah setelah dibuat). Mirip `readonly` di TypeScript.
- DTO memisahkan model database dari request/response.
- Tidak semua field dari entity perlu dikirim ke client.

---

## 11. Migration

### Konsep

Migration adalah cara EF Core mengubah schema database secara terstruktur. Mirip `prisma migrate dev` atau `doctrine:migrations:migrate` di Laravel.

### Perbandingan

| .NET (EF Core) | Prisma | Laravel (Doctrine) |
|----------------|--------|-------------------|
| `dotnet ef migrations add Name` | `prisma migrate dev --name` | `php artisan make:migration` |
| `dotnet ef database update` | `prisma db push` | `php artisan migrate` |
| `dotnet ef migrations remove` | `prisma migrate reset` | `php artisan migrate:rollback` |
| File generated di `Migrations/` | File generated di `prisma/migrations/` | File di `database/migrations/` |

### Contoh command

```bash
# Buat migration
dotnet ef migrations add InitialCreate

# Terapkan ke database
dotnet ef database update

# Hapus migration terakhir
dotnet ef migrations remove
```

### Penjelasan

- Migration pertama akan membuat tabel `__EFMigrationsHistory` untuk melacak migration yang sudah dijalankan.
- Error `Failed executing DbCommand` pada migration pertama adalah normal karena tabel history belum ada.
- Migration bisa di-rollback dengan `dotnet ef migrations remove`.

---

## 12. Configuration (appsettings.json)

### Konsep

`appsettings.json` adalah file konfigurasi utama di .NET. Mirip `.env` di Node.js atau `config/` di Laravel.

### Perbandingan

| .NET | Node.js | Laravel | NestJS |
|------|---------|---------|--------|
| `appsettings.json` | `.env` | `.env` | `config/` |
| `builder.Configuration` | `process.env` | `env()` | `ConfigService` |
| `GetConnectionString()` | `process.env.DATABASE_URL` | `env('DB_CONNECTION')` | `config.get()` |

### Contoh C#

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tickset;Username=<user>;Password=<password>"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

```csharp
// Akses di code
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

### Penjelasan

- `ConnectionStrings` → menyimpan koneksi ke database.
- `GetConnectionString("DefaultConnection")` → membaca connection string dari config.
- Jangan commit `appsettings.json` yang berisi password ke version control.
- Gunakan `appsettings.Development.json` untuk settings local.

## HTTP Status Code untuk Auth

| Status | Kapan dipakai | Laravel |
|--------|---------------|---------|
| 201 Created | Register sukses | `response()->created()` |
| 200 OK | Login sukses | `response()->json()` |
| 401 Unauthorized | Password salah / belum login | `abort(401)` |
| 409 Conflict | Email sudah terdaftar | `abort(409)` |

### Penjelasan

- Status code adalah kontrak antara server dan client — TanStack Query membaca status untuk menentukan apakah query sukses (`2xx`) atau error (`4xx/5xx`).
- 401 = "siapa kamu?" (tidak terautentikasi), 403 = "kamu tidak boleh" (terautentikasi tapi tidak berwenang).
- SELALU pakai status code yang benar, jangan 200 untuk semua error.

## JWT: Generate vs Validate (2 bagian terpisah)

### Sebelumnya (hanya generate)

```csharp
var token = tokenService.GenerateToken(user.Id, user.Email);
return Results.Ok(new { token });
```

Server **membuat** token, tapi tidak pernah **memeriksa** token yang dikirim client. Jadi endpoint mana pun tetap terbuka.

### Sesudahnya (tambah validate)

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* TokenValidationParameters */ });
// ...
app.UseAuthentication();
app.UseAuthorization();
```

Server sekarang **memeriksa** token di setiap request yang butuh auth.

### Kenapa 3 bagian?

| Bagian | Fungsi | Analogi Laravel |
|--------|--------|-----------------|
| `AddJwtBearer()` + `TokenValidationParameters` | **Daftarkan** cara cek token (secret, issuer, expiry) | `config/auth.php` guard `api` |
| `app.UseAuthentication()` | **Baca** token dari header `Authorization`, decode jadi `ClaimsPrincipal` | `AuthenticateSession` middleware |
| `app.UseAuthorization()` | **Putuskan** apakah route boleh diakses | `auth:api` middleware di route |

### Kenapa urutannya penting?

```csharp
app.UseAuthentication();  // 1. siapa kamu? (decode token → user)
app.UseAuthorization();   // 2. bolehkah? (cek policy/role)
```

Sama seperti Laravel: middleware `auth` jalan **sebelum** controller. Kalau urutan dibalik, authorization tidak punya "user" untuk dicek.

### Pipeline lengkap

```text
Request → Authentication (decode token) → Authorization (cek hak) → Endpoint
```

### Contoh di Laravel

```php
Route::middleware('auth:api')->group(function () {
    Route::get('/me', [UserController::class, 'me']);
});
```

```csharp
// .NET — endpoint terproteksi
app.MapGet("/api/me", ...).RequireAuthorization();
```

## Kenapa Service Harus Didaftarkan ke DI di Program.cs?

### Pertanyaan

"TokenService hanya generate token, kenapa perlu `builder.Services.AddScoped<TokenService>()`?"

### Jawaban

Pendaftaran DI **bukan** tentang apa yang dilakukan service, tapi tentang **bagaimana object-nya dibuat dan diambil**.

Endpoint butuh instance `TokenService`:

```csharp
app.MapPost("/api/auth/login", async (LoginDto dto, AuthService auth, TokenService token) => ...
//                                                                      ^^^^^^^^^^^^^^^
//                                            DI container harus tahu cara membuat object ini
```

Kalau tidak didaftarkan, DI container tidak tahu cara membuat `TokenService` → **runtime error**:

```
Unable to resolve service for type 'Tickset.Services.TokenService'
```

### Perbandingan Laravel

| | Laravel | .NET |
|--|---------|------|
| Concrete class | **Auto-resolve**, tidak perlu daftar | **Wajib daftar** manual |
| Interface | Daftar di Service Provider | Daftar manual |
| Container | `app()->make(TokenService::class)` | `ActivatorUtilities` / DI container |

```php
// Laravel — concrete class auto-resolve, tidak perlu bind
class AuthController {
    public function __construct(TokenService $token) {} // langsung jalan
}
```

```csharp
// .NET — WAJIB daftar dulu, baru bisa inject
builder.Services.AddScoped<TokenService>();  // wajib
app.MapPost(..., (..., TokenService token) => ...);  // baru bisa
```

**Ini perbedaan utama:** DI container .NET "ketat" — tidak ada yang di-implicit. Laravel container lebih longgar untuk concrete class.

### Kenapa Scoped?

```csharp
builder.Services.AddScoped<TokenService>();
//            ^^^^^^
```

| Lifetime | Object dibuat | Mirip Laravel |
|----------|---------------|---------------|
| `Transient` | Setiap inject baru object | — |
| `Scoped` | 1 object per request | request lifecycle |
| `Singleton` | 1 object selama app hidup | container singleton |

`Scoped` paling umum untuk service yang akses database (1 request = 1 `DbContext`). Karena semua service dalam satu request harus share `DbContext` yang sama, supaya perubahan data konsisten.

## Kesalahan Konsep Umum: DI ≠ Async

### Pertanyaan

"Perlu ditambah ke service agar berjalan async, juga agar bisa berbagi DbContext?"

### Jawaban: Keduanya bukan alasan pendaftaran DI

**1. Async tidak ada hubungannya dengan DI.**

Async datang dari `async/await` di dalam method:

```csharp
public async Task<User?> LoginAsync(LoginDto dto)
//     ^^^^^                                    <- ini yang bikin async
{
    var user = await _context.Users.FirstOrDefaultAsync(...);
    //           ^^^^^ EF Core versi async, non-blocking I/O
}
```

`AddScoped<TokenService>()` tidak membuat apa pun jadi async. Method tanpa `async` tetap sync meski service sudah terdaftar di DI.

**2. Berbagi DbContext adalah KONSEKUENSI dari lifetime `Scoped`, bukan tujuan pendaftaran.**

Tiga hal terpisah:

| Hal | Fungsi |
|-----|--------|
| `AddScoped<T>()` | Daftarkan ke container → supaya bisa di-inject |
| Lifetime `Scoped` | 1 object per request → semua service share 1 DbContext |
| `async/await` | Non-blocking I/O → thread tidak menunggu database |

```text
1 Request
├── AuthService (Scoped) ─────┐
├── TokenService (Scoped) ────┼── semua punya reference SAMA
├── ApplicationDbContext ─────┘   ke 1 instance DbContext
```

### Kenapa share DbContext penting?

```csharp
await _authService.RegisterAsync(dto);   // DbContext: Users.Add(user)
await _ticketService.CreateTicketAsync(); // DbContext: masih ada track "user" yang sama
```

Kalau tiap service punya `DbContext` sendiri (misal pakai `Singleton`), perubahan di service A tidak terlihat di service B → data tidak konsisten.

**Mirip Laravel:** 1 request = 1 database transaction/session. Eloquent otomatis share connection dalam satu request.

### Ringkasan

- **Daftar DI** → supaya container bisa membuat & menginject object
- **Lifetime Scoped** → supaya share DbContext per request
- **async/await** → supaya I/O tidak memblokir thread (hubungannya di kode method, bukan di Program.cs)

## Tujuan Utama DI Registration (ringkas)

```csharp
builder.Services.AddScoped<AuthService>();
```

Dua tujuan saja:

1. **Otomatis membuat object** — endpoint menulis `AuthService auth` sebagai parameter, container membuat instance-nya (isi `ApplicationDbContext` ikut di-inject). Anda tidak perlu `new AuthService(context)` manual.
2. **Mengatur umur object** — `Scoped` = hidup 1 request, lalu dibuang. Menentukan apakah object dibuat baru tiap inject (`Transient`), per request (`Scoped`), atau sekali saja (`Singleton`).

```csharp
// Tanpa DI — manual, ribet, salah lifetime
var db = new DbContext(options);
var service = new AuthService(db);
var token = new TokenService(config);

// Dengan DI — tulis parameter saja, container yang urus
app.MapPost("/api/auth/login", async (LoginDto dto, AuthService auth, TokenService token) => ...);
```

**Mirip Laravel Service Provider:** `AppServiceProvider::register()` → `$this->app->bind(...)` — tujuannya sama: memberi tahu container cara membuat & kapan membuat object.

## Token: Kapan Generate vs Kapan Baca

### Kesalahan

Endpoint `/me` memanggil `token.GenerateToken()` lagi — padahal token sudah dibuat saat login.

### Aturannya

| Aksi | Kapan | Siapa |
|------|-------|-------|
| **Generate** token | Login / Register / Refresh | Server, saat mengeluarkan kredensial |
| **Baca** (decode) token | Setiap request terproteksi | Server, dari header `Authorization` |

```text
Login        → server HASILKAN token  → client SIMPAN
Setiap call  → client KIRIM token (Authorization: Bearer xxx) → server BACA claims
/me          → server BACA userId/email dari token → return. TIDAK generate baru.
```

### Kenapa /me tidak boleh generate token baru?

1. **Token sudah ada di request** — `ClaimsPrincipal user` sudah berisi isi token (userId, email). Tinggal baca.
2. **Token jadi tidak pernah expired** — kalau tiap request dapat token baru, `exp` 1 jam percuma. User tidak akan pernah logout.
3. **Sia-sia** — client sudah punya token, mengapa dikasih lagi?

### Perbandingan Laravel

```php
// Laravel — /me cukup BACA user dari token, tidak buat token baru
Route::middleware('auth:api')->get('/me', function (Request $request) {
    return $request->user();  // baca, bukan generate
});
```

```csharp
// .NET — sama: baca claims, jangan GenerateToken()
app.MapGet("/api/auth/me", (ClaimsPrincipal user) => ...)
```

### Analogi sederhana

- Token = **tiket masuk**. Login = dapat tiket. Setiap masuk gedung = tunjukkan tiket (baca), bukan dicetak tiket baru.

## DTO per Arah dan per Endpoint

### Pertanyaan

"AuthResponseDto ada field Token, tapi endpoint /me tidak generate token — pakai DTO apa?"

### Jawaban: beda endpoint = beda DTO response

| Endpoint | Task | Response DTO |
|----------|------|--------------|
| `POST /auth/register` | Terbitkan token baru | `AuthResponseDto(Token, UserId, Email)` |
| `POST /auth/login` | Terbitkan token baru | `AuthResponseDto(Token, UserId, Email)` |
| `GET /auth/me` | Baca token lama saja | `MeResponseDto(UserId, Email)` — tanpa Token |

```csharp
// DTOs/AuthResponseDto.cs — hanya untuk login/register
public record AuthResponseDto(string Token, Guid UserId, string Email);

// DTOs/MeResponseDto.cs — untuk endpoint yang tidak menerbitkan token
public record MeResponseDto(Guid UserId, string Email);
```

### Kenapa tidak satu DTO untuk semua?

Kalau `AuthResponseDto` dipaksa untuk `/me`, ada 2 jalan buruk:
1. **Generate token palsu** di `/me` → token tidak pernah expired (sudah dibahas sebelumnya)
2. **Isi Token = string kosong** → client bingung, field yang tidak berguna

**Prinsip:** DTO menggambarkan **apa yang benar-benar dikirim endpoint itu** — bukan semua data yang pernah ada.

### Perbandingan Laravel

```php
// Laravel — beda endpoint beda resource/array
return response()->json(['access_token' => $token, 'user' => $user]); // login
return response()->json($user);                                        // /me
```

## DTO Wajib atau Anonymous Object?

### Pertanyaan

"Kenapa perlu buat DTO khusus untuk /me, bukankah bisa pakai object biasa?"

### Jawaban

Bisa keduanya, tapi konvensi project (AGENTS.md): **DTO = kontrak request/response**.

```csharp
// Boleh — anonymous object (ringkas, tapi tidak ada tipe)
return Results.Ok(new { userId, email });

// Dianjurkan — DTO (ada tipe, bisa di-reuse, client tahu bentuknya)
return Results.Ok(new MeResponseDto(userId, email));
```

Kapan DTO wajib/penting:
1. **Respons dipakai di banyak endpoint** → pakai 1 DTO, jangan duplikat anonymous
2. **Client butuh tipe** → TanStack Query/TypeScript bisa generate tipe dari OpenAPI
3. **Field banyak / bersarang** → anonymous object mudah salah ketik, DTO di-compile check

Kapan anonymous cukup:
- Response kecil, sekali pakai, tidak di-reuse.

Untuk `/me`: 2 field, tapi dibuat DTO karena konvensi project — ringan, 1 baris.

## Error: ArgumentNullException di JwtBearer Options

### Gejala

Login sukses (token tergenerate), tapi semua request gagal 500:

```
System.ArgumentNullException: Value cannot be null. (Parameter 's')
   at System.Text.Encoding.GetBytes(String s)
   at Program.cs:line 24 (AddJwtBearer options)
```

### Penyebab

Key yang dibaca DI CONTOH berbeda dengan key yang ada di `appsettings.json`:

```csharp
// Program.cs — membaca "Jwt:Key"
builder.Configuration["Jwt:Key"]        // ← null! di config tidak ada "Key"

// appsettings.json — field-nya "Secret"
"Jwt": { "Secret": "..." }             // ← ada "Secret", bukan "Key"
```

`Encoding.UTF8.GetBytes(null)` → melempar `ArgumentNullException`.

**Kenapa login tetap jalan?** Karena `TokenService` membaca `Jwt:Secret` (benar). Hanya `AddJwtBearer` (validasi) yang salah baca.

### Pelajaran penting

1. **Configuration key salah = tidak di-compile check.** `builder.Configuration["Jwt:Key"]` tetap meng-compile walau key tidak ada — nilainya `null`. Error baru muncul saat runtime. (Beda dengan typo nama class/variable yang langsung error saat build.)
2. **Dua tempat baca config harus konsisten** — `TokenService` (generate) dan `AddJwtBearer` (validate) harus pakai key yang sama.
3. **`!` (null-forgiving) menyembunyikan peringatan, bukan masalah.** `_config["Jwt:Secret"]!` artinya "percaya ini tidak null" — kalau ternyata null, tetap crash di runtime.

### Perbandingan Laravel

```php
// config/jwt.php + .env — konsistensi key
config('jwt.secret');  // kalau .env kosong → error juga, tapi saat boot

// .NET — configuration lookup tidak divalidasi saat boot
builder.Configuration["Jwt:Key"];  // null, tidak ada error sampai dipakai
```

### Fix

Samakan key: `Jwt:Key` → `Jwt:Secret` (atau ubah `appsettings.json`).

## Checkpoint Phase 1 — Hasil Test Auth

| Skenario | Status | HTTP |
|----------|--------|------|
| Login (password benar) | ✅ | 200 + token |
| `/me` tanpa token | ✅ | 401 |
| `/me` dengan token valid | ✅ | 200 + claims |
| `/me` token palsu | ✅ | 401 |

**Flow yang sudah jalan:**

```text
POST /auth/login → AuthService (cek password) → TokenService (generate JWT)
       ↓
GET/POST /auth/me + header "Authorization: Bearer <jwt>"
       ↓
UseAuthentication (decode JWT → ClaimsPrincipal) → RequireAuthorization (ada token? ✓)
       ↓
Endpoint baca claims → return data user
```

## JWT Stateless: 4 Pertanyaan Penting

### 1. Kenapa token tidak disimpan di model/database?

JWT bersifat **stateless** — semua data sudah ada DI DALAM token (userId, email, expiry) dan tanda tangan kriptografis. Server tidak perlu mencari token di database.

```text
JWT = envelope yang sudah disegel:
{ userId, email, exp }  →  di-HMAC dengan secret  →  signature
```

| Pendekatan | Contoh Laravel | Simpan di DB? |
|------------|----------------|---------------|
| **Stateless JWT** (pakai kita) | JWT package | ❌ Tidak |
| **Token di DB** | Laravel Sanctum personal access tokens | ✅ Ya |
| **Session** | Laravel session driver database | ✅ Ya |

**Trade-off stateless:**
- ✅ Setiap request tidak perlu query database → cepat, mudah scale
- ❌ Token tidak bisa di-revoke sebelum expired (logout global/blokir user susah)
- Solusi nanti: **refresh token** yang disimpan di DB, atau token blacklist

### 2. Kenapa `/me` tidak konsisten dengan `AuthResponseDto`?

Karena **konsistensi bukan tujuan, kontrak yang benar adalah tujuan.**

- Login/register → **menerbitkan** token baru → response harus ada token
- `/me` → **membaca** token yang SUDAH dikirim client di header request → tidak ada token baru untuk dikembalikan

Kalau `/me` ikut return token berarti setiap request menghasilkan token baru → token tidak pernah expired (sudah dibahas). DTO berbeda karena isi response berbeda — itu justru konsisten secara *meaning*.

### 3. Kenapa expiry tidak di-set di model?

**Expiry SUDAH di-set** — di dalam token, bukan di database:

```csharp
// TokenService.cs
expires: DateTime.UtcNow.AddHours(1)   // ← ini
```

Isi token (payload):
```json
{
  "identifier": "01a10f37-...",
  "emailaddress": "andi@email.com",
  "exp": 1791291211,        ← timestamp expiry, di-encode ke token
  "iss": "TicksetApi",
  "aud": "TicksetClient"
}
```

**Kenapa tidak disimpan di model User?** Karena expiry adalah properti dari **token**, bukan dari user. User bisa punya banyak token (banyak device), tiap token punya expiry sendiri.

### 4. Kalau token tidak disimpan, bagaimana cara cek valid?

**Token mengecek dirinya sendiri secara matematis — tanpa database:**

```text
Request masuk dengan "Authorization: Bearer xxx"
        ↓
1. Split token → header.payload.signature
2. Hitung ULANG: HMAC(header.payload, SECRET)
3. Bandingkan hasil dengan signature yang dikirim
   - Sama   → token asli, tidak dimanipulasi ✅
   - Beda   → token palsu ❌ → 401
4. Cek "exp" di payload vs waktu sekarang
   - belum lewat → masih berlaku ✅
   - sudah lewat → 401 expired
```

Konfigurasi validasi di `Program.cs`:
```csharp
ValidateIssuerSigningKey = true,   // cek signature dengan secret
ValidateLifetime = true,            // cek exp
```

**Ini analogi tanda tangan digital:** siapa pun boleh membaca isi surat (payload), tapi hanya yang punya SECRET bisa membuat signature yang benar. Memalsukan isi surat membuat signature cocok → ketahuan.

**Kenapa jadi tidak perlu generate token baru?**
Client menyimpan token hasil login, mengirim token YANG SAMA di setiap request. Server cukup **memverifikasi** (murni hitungan, no DB), bukan membuat baru. Token baru hanya dibuat saat login ulang / token expired.

### Kenapa validasi tidak butuh query database?

Karena semua yang dibutuhkan ada di token:
- **Siapa** → claim `identifier`
- **Masih berlaku?** → claim `exp`
- **Asli?** → signature vs secret

Bandingkan Laravel Sanctum: cek token = query table `personal_access_tokens` (butuh DB). JWT: cek token = hitungan HMAC (tanpa DB).

## Ringkasan Pemahaman JWT yang Benar

```text
1. LOGIN      → server generate JWT (sekali) → kirim ke client
2. SIMPAN     → client yang menyimpan (localStorage / cookie / memory)
3. KIRIM      → client lampirkan di header "Authorization: Bearer <jwt>" setiap request
4. VALIDASI   → server verifikasi signature + expiry (HITUNGAN, tanpa query DB)
5. BARU LAGI  → hanya saat login ulang atau token expired
```

### Koreksi kecil atas pemahaman "frontend urus penyimpanan"

1. **Betul** — penyimpanan di client. Tapi *di mana* client menyimpan itu penting:
   - `localStorage` → rentan XSS (script jahat bisa baca)
   - `HttpOnly cookie` → aman dari XSS, tapi perlu mitigasi CSRF
   - in-memory (state React/TanStack) → aman, tapi hilang saat refresh halaman
2. **Betul** — backend cukup validasi tanpa simpan token ke DB.
   Tapi backend TETAP menyimpan **secret** (di `appsettings.json`) — tanpa secret, validasi tidak mungkin.
3. **Nuansa** — "stateless" itu *default*, bukan aturan mutlak. Server BOLEH menyimpan token jika butuh revocation (logout semua device, blokir user) → itu namanya blacklist/refresh token table.

## Keamanan Config di .NET: Jangan Commit Secret

### Hierarki konfigurasi (dibaca dari bawah ke atas, yang bawah menang)

```text
appsettings.json              ← boleh di-commit, ISI TANPA secret
appsettings.{Environment}.json
User Secrets (dev only)       ← secret lokal, DI LUAR folder project, tidak pernah ter-commit
Environment variables         ← production (Docker/K8s/CI)
```

### User Secrets (perintah)

```bash
dotnet user-secrets init                              # sekali saja, tambah UserSecretsId ke csproj
dotnet user-secrets set "Jwt:Key" "rahasia..."        # simpan nilai
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."
dotnet user-secrets list                              # lihat
```

Penyimpanan Windows: `%APPAPPDATA%\microsoft\UserSecrets\{id}\secrets.json`
→ di LUAR repo, jadi mustahil ter-commit. Otomatis terbaca saat `ASPNETCORE_ENVIRONMENT=Development`.

### Perbandingan Laravel

| Laravel | .NET |
|---------|------|
| `.env` (di-gitignore) | User Secrets (dev) / env vars (prod) |
| `config('key')` baca `.env` | `builder.Configuration["Jwt:Key"]` |
| `.env.example` (template, di-commit) | `appsettings.json` kosong (template, di-commit) |

**Konsep sama:** `.env` tidak di-commit, `.env.example` di-commit. `appsettings.json` berisi struktur kosong, nilai rahasia di secret store.

### .gitignore .NET yang wajib ada

```
[Bb]in/            # hasil build
[Oo]bj/            # file intermediate
.vs/               # Visual Studio
.idea/             # Rider
*.user, *.suo      # file user-specific
TestResults/       # hasil test/coverage
*.local.json       # override config lokal
.env, .env.*       # environment file
```

### Lessons learned (repo ini)

1. Secret yang sudah ter-commit tetap ada di **history** — menghapus di file terbaru tidak cukup.
2. Fix: rewrite history (`git filter-repo --replace-text`) + force push, lalu bersihkan object lama di GitHub (GC / recreate repo).
3. Selalu scan sebelum push: `git log -S "password" --oneline`
