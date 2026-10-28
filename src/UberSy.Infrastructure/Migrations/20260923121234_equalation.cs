using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace UberSy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Equalation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BannedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Boundary = table.Column<Point>(type: "geography", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Floor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbAddres__3214EC07BCC06044", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbDeliveryPricingRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleType = table.Column<byte>(type: "tinyint", nullable: false),
                    ServiceTier = table.Column<byte>(type: "tinyint", nullable: false),
                    CarType = table.Column<byte>(type: "tinyint", nullable: true),
                    CustomerRatePerKm = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DriverRatePerKm = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DriverRatePerOrder = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MaxDistanceKm = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    StartHour = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndHour = table.Column<TimeOnly>(type: "time", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbDelive__3214EC0737513A98", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbDriver",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleType = table.Column<byte>(type: "tinyint", nullable: false),
                    ServiceTier = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    CarType = table.Column<byte>(type: "tinyint", nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PlateNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbDriver__3214EC0703EA4F36", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbRestaurant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CoverUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CommissionRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    OpeningTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    ClosingTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MaxOrderAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsOpenNow = table.Column<bool>(type: "bit", nullable: false),
                    Boundary = table.Column<Point>(type: "geography", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    AcceptReservation = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbRestau__3214EC071BFEDFB1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbServiceRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceType = table.Column<byte>(type: "tinyint", nullable: false),
                    TrackingNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbServic__3214EC07831A20B8", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbServiceZone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Boundary = table.Column<Geometry>(type: "geography", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbServic__3214EC078865692F", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbTag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbTag__3214EC072088A8E9", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbDriverLocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Boundary = table.Column<Point>(type: "geography", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbDriver__3214EC079E4D59BF", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbDriverLocation_Driver",
                        column: x => x.DriverId,
                        principalTable: "TbDriver",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbDriverWallet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbDriver__3214EC07F451DB98", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbDriverWallet_Driver",
                        column: x => x.DriverId,
                        principalTable: "TbDriver",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbAddOn",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbAddOn__3214EC070FA8160A", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbAddOn_Restaurant",
                        column: x => x.RestaurantId,
                        principalTable: "TbRestaurant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbDiscount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Scope = table.Column<byte>(type: "tinyint", nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MaxUsageCount = table.Column<int>(type: "int", nullable: true),
                    MaxUsagePerCustomer = table.Column<byte>(type: "tinyint", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbDiscou__3214EC07F5023E34", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbDiscount_Restaurant",
                        column: x => x.RestaurantId,
                        principalTable: "TbRestaurant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbPrinter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConnectionInfo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbPrinte__3214EC071CD9E703", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbPrinter_Restaurant",
                        column: x => x.RestaurantId,
                        principalTable: "TbRestaurant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbRestaurantImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SortOrder = table.Column<byte>(type: "tinyint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbRestau__3214EC0721745D85", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbRestaurantImage_Restaurant",
                        column: x => x.RestaurantId,
                        principalTable: "TbRestaurant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbComplaint",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbCompla__3214EC07140EB01E", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbComplaint_ServiceRequest",
                        column: x => x.ServiceRequestId,
                        principalTable: "TbServiceRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbPayment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Method = table.Column<byte>(type: "tinyint", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    PaidAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbPaymen__3214EC0790777B2D", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbPayment_ServiceRequest",
                        column: x => x.ServiceRequestId,
                        principalTable: "TbServiceRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Rating = table.Column<byte>(type: "tinyint", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbReview__3214EC079BD3F549", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbReview_Driver",
                        column: x => x.DriverId,
                        principalTable: "TbDriver",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbReview_Restaurant",
                        column: x => x.RestaurantId,
                        principalTable: "TbRestaurant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbReview_ServiceRequest",
                        column: x => x.ServiceRequestId,
                        principalTable: "TbServiceRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbRideRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedVehicleType = table.Column<byte>(type: "tinyint", nullable: false),
                    RequestedServiceTier = table.Column<byte>(type: "tinyint", nullable: false),
                    RequestedCarType = table.Column<byte>(type: "tinyint", nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceiverPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PickupBoundary = table.Column<Point>(type: "geography", nullable: false),
                    DropoffBoundary = table.Column<Point>(type: "geography", nullable: false),
                    DistanceKm = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DriverEarning = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbRideRe__3214EC07B4A5C518", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbRideRequest_Driver",
                        column: x => x.DriverId,
                        principalTable: "TbDriver",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbRideRequest_ServiceRequest",
                        column: x => x.Id,
                        principalTable: "TbServiceRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbServiceRequestHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbServic__3214EC07642341B0", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbServiceRequestHistory_ServiceRequest",
                        column: x => x.ServiceRequestId,
                        principalTable: "TbServiceRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbServiceRequestOffer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    RespondedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbServic__3214EC07D247AFFA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbServiceRequestOffer_Driver",
                        column: x => x.DriverId,
                        principalTable: "TbDriver",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbServiceRequestOffer_ServiceRequest",
                        column: x => x.ServiceRequestId,
                        principalTable: "TbServiceRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbTableReservation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PartySize = table.Column<int>(type: "int", nullable: false),
                    DepositAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbTableR__3214EC07DB205D72", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbTableReservation_Restaurant",
                        column: x => x.RestaurantId,
                        principalTable: "TbRestaurant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbTableReservation_ServiceRequest",
                        column: x => x.Id,
                        principalTable: "TbServiceRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbRestaurantTag",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbRestaurantTag", x => new { x.RestaurantId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TbRestaurantTag_Restaurant",
                        column: x => x.RestaurantId,
                        principalTable: "TbRestaurant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbRestaurantTag_Tag",
                        column: x => x.TagId,
                        principalTable: "TbTag",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbDriverWalletTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverWalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcessedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbDriver__3214EC079D1AF900", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbDriverWalletTransaction_ServiceRequest",
                        column: x => x.ServiceRequestId,
                        principalTable: "TbServiceRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbDriverWalletTransaction_Wallet",
                        column: x => x.DriverWalletId,
                        principalTable: "TbDriverWallet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbDiscountUsage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbDiscou__3214EC07A1000626", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbDiscountUsage_Discount",
                        column: x => x.DiscountId,
                        principalTable: "TbDiscount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbDiscountUsage_ServiceRequest",
                        column: x => x.ServiceRequestId,
                        principalTable: "TbServiceRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbFoodOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DriverEarning = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PlatformCommissionFromRestaurant = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbFoodOr__3214EC0715E2F3EE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbFoodOrder_Address",
                        column: x => x.AddressId,
                        principalTable: "TbAddress",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbFoodOrder_Discount",
                        column: x => x.DiscountId,
                        principalTable: "TbDiscount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbFoodOrder_Driver",
                        column: x => x.DriverId,
                        principalTable: "TbDriver",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbFoodOrder_Restaurant",
                        column: x => x.RestaurantId,
                        principalTable: "TbRestaurant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbFoodOrder_ServiceRequest",
                        column: x => x.Id,
                        principalTable: "TbServiceRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrinterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SortOrder = table.Column<byte>(type: "tinyint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbCatego__3214EC07EC94C04F", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbCategory_Printer",
                        column: x => x.PrinterId,
                        principalTable: "TbPrinter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbCategory_Restaurant",
                        column: x => x.RestaurantId,
                        principalTable: "TbRestaurant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbComplaintMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComplaintId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbCompla__3214EC075DB39A21", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbComplaintMessage_Complaint",
                        column: x => x.ComplaintId,
                        principalTable: "TbComplaint",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SortOrder = table.Column<byte>(type: "tinyint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbItem__3214EC0723E01AA4", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbItem_Category",
                        column: x => x.CategoryId,
                        principalTable: "TbCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbItem_Restaurant",
                        column: x => x.RestaurantId,
                        principalTable: "TbRestaurant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbDiscountItem",
                columns: table => new
                {
                    DiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbDiscountItem", x => new { x.DiscountId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_TbDiscountItem_Discount",
                        column: x => x.DiscountId,
                        principalTable: "TbDiscount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbDiscountItem_Item",
                        column: x => x.ItemId,
                        principalTable: "TbItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbItemAddOn",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddOnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbItemAddOn", x => new { x.ItemId, x.AddOnId });
                    table.ForeignKey(
                        name: "FK_TbItemAddOn_AddOn",
                        column: x => x.AddOnId,
                        principalTable: "TbAddOn",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbItemAddOn_Item",
                        column: x => x.ItemId,
                        principalTable: "TbItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbOrderItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbOrderI__3214EC075C0837FA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbOrderItem_FoodOrder",
                        column: x => x.FoodOrderId,
                        principalTable: "TbFoodOrder",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbOrderItem_Item",
                        column: x => x.ItemId,
                        principalTable: "TbItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TbOrderItemAddOn",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddOnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbOrderI__3214EC07C054362B", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbOrderItemAddOn_AddOn",
                        column: x => x.AddOnId,
                        principalTable: "TbAddOn",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TbOrderItemAddOn_OrderItem",
                        column: x => x.OrderItemId,
                        principalTable: "TbOrderItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TbAddOn_RestaurantId",
                table: "TbAddOn",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_TbCategory_PrinterId",
                table: "TbCategory",
                column: "PrinterId");

            migrationBuilder.CreateIndex(
                name: "IX_TbCategory_RestaurantId",
                table: "TbCategory",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_TbComplaint_ServiceRequestId",
                table: "TbComplaint",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TbComplaintMessage_ComplaintId",
                table: "TbComplaintMessage",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_TbDiscount_RestaurantId",
                table: "TbDiscount",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "UQ_TbDiscount_Code",
                table: "TbDiscount",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TbDiscountItem_ItemId",
                table: "TbDiscountItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TbDiscountUsage_DiscountId",
                table: "TbDiscountUsage",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_TbDiscountUsage_ServiceRequestId",
                table: "TbDiscountUsage",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "UQ_TbDriver_UserId",
                table: "TbDriver",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_TbDriverLocation_DriverId",
                table: "TbDriverLocation",
                column: "DriverId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_TbDriverWallet_DriverId",
                table: "TbDriverWallet",
                column: "DriverId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TbDriverWalletTransaction_DriverWalletId",
                table: "TbDriverWalletTransaction",
                column: "DriverWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_TbDriverWalletTransaction_ServiceRequestId",
                table: "TbDriverWalletTransaction",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TbFoodOrder_AddressId",
                table: "TbFoodOrder",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_TbFoodOrder_DiscountId",
                table: "TbFoodOrder",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_TbFoodOrder_DriverId",
                table: "TbFoodOrder",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_TbFoodOrder_RestaurantId",
                table: "TbFoodOrder",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_TbItem_CategoryId",
                table: "TbItem",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TbItem_Restaurant_Active",
                table: "TbItem",
                columns: new[] { "RestaurantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_TbItem_RestaurantId",
                table: "TbItem",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_TbItemAddOn_AddOnId",
                table: "TbItemAddOn",
                column: "AddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_TbOrderItem_FoodOrderId",
                table: "TbOrderItem",
                column: "FoodOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TbOrderItem_ItemId",
                table: "TbOrderItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TbOrderItemAddOn_AddOnId",
                table: "TbOrderItemAddOn",
                column: "AddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_TbOrderItemAddOn_OrderItemId",
                table: "TbOrderItemAddOn",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TbPayment_ServiceRequestId",
                table: "TbPayment",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TbPrinter_RestaurantId",
                table: "TbPrinter",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_TbRestaurant_OwnerId",
                table: "TbRestaurant",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TbRestaurantImage_RestaurantId",
                table: "TbRestaurantImage",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_TbRestaurantTag_TagId",
                table: "TbRestaurantTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TbReview_DriverId",
                table: "TbReview",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_TbReview_RestaurantId",
                table: "TbReview",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_TbReview_ServiceRequestId",
                table: "TbReview",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TbRideRequest_DriverId",
                table: "TbRideRequest",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_TbServiceRequest_Customer_Status_CreatedAt",
                table: "TbServiceRequest",
                columns: new[] { "CustomerId", "Status", "CreatedAtUtc" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_TbServiceRequestHistory_ServiceRequestId",
                table: "TbServiceRequestHistory",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TbServiceRequestOffer_DriverId",
                table: "TbServiceRequestOffer",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_TbServiceRequestOffer_ServiceRequestId",
                table: "TbServiceRequestOffer",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTableReservation_RestaurantId",
                table: "TbTableReservation",
                column: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "TbComplaintMessage");

            migrationBuilder.DropTable(
                name: "TbDeliveryPricingRule");

            migrationBuilder.DropTable(
                name: "TbDiscountItem");

            migrationBuilder.DropTable(
                name: "TbDiscountUsage");

            migrationBuilder.DropTable(
                name: "TbDriverLocation");

            migrationBuilder.DropTable(
                name: "TbDriverWalletTransaction");

            migrationBuilder.DropTable(
                name: "TbItemAddOn");

            migrationBuilder.DropTable(
                name: "TbOrderItemAddOn");

            migrationBuilder.DropTable(
                name: "TbPayment");

            migrationBuilder.DropTable(
                name: "TbRestaurantImage");

            migrationBuilder.DropTable(
                name: "TbRestaurantTag");

            migrationBuilder.DropTable(
                name: "TbReview");

            migrationBuilder.DropTable(
                name: "TbRideRequest");

            migrationBuilder.DropTable(
                name: "TbServiceRequestHistory");

            migrationBuilder.DropTable(
                name: "TbServiceRequestOffer");

            migrationBuilder.DropTable(
                name: "TbServiceZone");

            migrationBuilder.DropTable(
                name: "TbTableReservation");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TbComplaint");

            migrationBuilder.DropTable(
                name: "TbDriverWallet");

            migrationBuilder.DropTable(
                name: "TbAddOn");

            migrationBuilder.DropTable(
                name: "TbOrderItem");

            migrationBuilder.DropTable(
                name: "TbTag");

            migrationBuilder.DropTable(
                name: "TbFoodOrder");

            migrationBuilder.DropTable(
                name: "TbItem");

            migrationBuilder.DropTable(
                name: "TbAddress");

            migrationBuilder.DropTable(
                name: "TbDiscount");

            migrationBuilder.DropTable(
                name: "TbDriver");

            migrationBuilder.DropTable(
                name: "TbServiceRequest");

            migrationBuilder.DropTable(
                name: "TbCategory");

            migrationBuilder.DropTable(
                name: "TbPrinter");

            migrationBuilder.DropTable(
                name: "TbRestaurant");
        }
    }
}
