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
    "DefaultConnection": "Host=localhost;Port=5432;Database=tickset;Username=REMOVED;Password=REMOVED"
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
