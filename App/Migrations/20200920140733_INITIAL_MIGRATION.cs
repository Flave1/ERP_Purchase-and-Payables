using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puchase_and_payables.Migrations
{
    public partial class INITIAL_MIGRATION : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmEmailCodes",
                columns: table => new
                {
                    cConfirmEmailCodeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ConfirnamationTokenCode = table.Column<string>(nullable: true),
                    IssuedDate = table.Column<DateTime>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmEmailCodes", x => x.cConfirmEmailCodeId);
                });

            migrationBuilder.CreateTable(
                name: "cor_approvaldetail",
                columns: table => new
                {
                    ApprovalDetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    StaffId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    TargetId = table.Column<int>(nullable: false),
                    ReferredStaffId = table.Column<int>(nullable: false),
                    WorkflowToken = table.Column<string>(nullable: true),
                    ArrivalDate = table.Column<DateTime>(nullable: false),
                    ResponseDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_approvaldetail", x => x.ApprovalDetailId);
                });

            migrationBuilder.CreateTable(
                name: "cor_bankaccountdetail",
                columns: table => new
                {
                    BankAccountDetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    AccountName = table.Column<string>(nullable: true),
                    AccountNumber = table.Column<string>(nullable: true),
                    BVN = table.Column<string>(nullable: true),
                    Bank = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_bankaccountdetail", x => x.BankAccountDetailId);
                });

            migrationBuilder.CreateTable(
                name: "cor_bid_and_tender",
                columns: table => new
                {
                    BidAndTenderId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    LPOnumber = table.Column<string>(nullable: true),
                    PLPOId = table.Column<int>(nullable: false),
                    RequestingDepartment = table.Column<int>(nullable: false),
                    DescriptionOfRequest = table.Column<string>(nullable: true),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    Suppliernumber = table.Column<string>(nullable: true),
                    SupplierName = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    AmountApproved = table.Column<decimal>(nullable: false),
                    ProposalTenderUploadName = table.Column<string>(nullable: true),
                    ProposalTenderUploadPath = table.Column<string>(nullable: true),
                    ProposalTenderUploadFullPath = table.Column<string>(nullable: true),
                    ProposalTenderUploadType = table.Column<string>(nullable: true),
                    Extention = table.Column<string>(nullable: true),
                    ProposedAmount = table.Column<decimal>(nullable: false),
                    DateSubmitted = table.Column<DateTime>(nullable: false),
                    DecisionResult = table.Column<int>(nullable: false),
                    ApprovalStatusId = table.Column<int>(nullable: false),
                    WorkflowToken = table.Column<string>(nullable: true),
                    SupplierAddress = table.Column<string>(nullable: true),
                    ExpectedDeliveryDate = table.Column<DateTime>(nullable: false),
                    PurchaseReqNoteId = table.Column<int>(nullable: false),
                    IsRejected = table.Column<bool>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    SelectedSuppliers = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_bid_and_tender", x => x.BidAndTenderId);
                });

            migrationBuilder.CreateTable(
                name: "cor_financialdetail",
                columns: table => new
                {
                    FinancialdetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    BusinessSize = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    SupplierId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_financialdetail", x => x.FinancialdetailId);
                });

            migrationBuilder.CreateTable(
                name: "cor_identification",
                columns: table => new
                {
                    IdentificationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    IsCorporate = table.Column<bool>(nullable: false),
                    RegistrationNumber = table.Column<string>(nullable: true),
                    IncorporationDate = table.Column<DateTime>(nullable: true),
                    BusinessType = table.Column<int>(nullable: false),
                    OtherBusinessType = table.Column<string>(nullable: true),
                    Identification = table.Column<int>(nullable: false),
                    Identification_Number = table.Column<string>(nullable: true),
                    Expiry_Date = table.Column<DateTime>(nullable: true),
                    Nationality = table.Column<int>(nullable: false),
                    HaveWorkPermit = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_identification", x => x.IdentificationId);
                });

            migrationBuilder.CreateTable(
                name: "cor_serviceterms",
                columns: table => new
                {
                    ServiceTermsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    Header = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_serviceterms", x => x.ServiceTermsId);
                });

            migrationBuilder.CreateTable(
                name: "cor_suppliertype",
                columns: table => new
                {
                    SupplierTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierTypeName = table.Column<string>(maxLength: 250, nullable: false),
                    TaxApplicable = table.Column<string>(nullable: true),
                    CreditGL = table.Column<int>(nullable: false),
                    DebitGL = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_suppliertype", x => x.SupplierTypeId);
                });

            migrationBuilder.CreateTable(
                name: "cor_taxsetup",
                columns: table => new
                {
                    TaxSetupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    TaxName = table.Column<string>(nullable: true),
                    Percentage = table.Column<double>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    SubGL = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_taxsetup", x => x.TaxSetupId);
                });

            migrationBuilder.CreateTable(
                name: "cor_topclient",
                columns: table => new
                {
                    TopClientId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Address = table.Column<string>(maxLength: 550, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    PhoneNo = table.Column<string>(maxLength: 50, nullable: false),
                    ContactPerson = table.Column<string>(maxLength: 50, nullable: true),
                    NoOfStaff = table.Column<int>(nullable: true),
                    CountryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_topclient", x => x.TopClientId);
                });

            migrationBuilder.CreateTable(
                name: "inv_invoice",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    LPOId = table.Column<int>(nullable: false),
                    LPONumber = table.Column<string>(nullable: true),
                    AmountPayable = table.Column<decimal>(nullable: false),
                    PaymentTermId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DeliveryDate = table.Column<DateTime>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    WorkflowToken = table.Column<string>(nullable: true),
                    ApprovalStatusId = table.Column<int>(nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    DateInvoiceGenerated = table.Column<DateTime>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    CreditGl = table.Column<int>(nullable: false),
                    DebitGl = table.Column<int>(nullable: false),
                    PaymentBankId = table.Column<int>(nullable: false),
                    SupplierBankId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inv_invoice", x => x.InvoiceId);
                });

            migrationBuilder.CreateTable(
                name: "purch_invoice",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    LPOId = table.Column<int>(nullable: false),
                    LPONumber = table.Column<string>(nullable: true),
                    AmountPayable = table.Column<decimal>(nullable: false),
                    PaymentTermIds = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DeliveryDate = table.Column<DateTime>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    GrossAmount = table.Column<decimal>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purch_invoice", x => x.InvoiceId);
                });

            migrationBuilder.CreateTable(
                name: "purch_requisitionnote",
                columns: table => new
                {
                    PurchaseReqNoteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    RequestBy = table.Column<string>(maxLength: 255, nullable: false),
                    PRNNumber = table.Column<string>(nullable: true),
                    DepartmentId = table.Column<int>(nullable: false),
                    DocumentNumber = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    IsFundAvailable = table.Column<bool>(nullable: true),
                    DeliveryLocation = table.Column<string>(maxLength: 1000, nullable: true),
                    StaffId = table.Column<int>(nullable: false),
                    Total = table.Column<decimal>(type: "money", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "date", nullable: true),
                    ApprovalStatusId = table.Column<int>(nullable: false),
                    WorkflowToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purch_requisitionnote", x => x.PurchaseReqNoteId);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Token = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    JwtId = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    Used = table.Column<bool>(nullable: false),
                    Invalidated = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
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
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
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
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
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
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
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
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
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
                name: "cor_paymentterms",
                columns: table => new
                {
                    PaymentTermId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    LPOId = table.Column<int>(nullable: false),
                    Phase = table.Column<int>(nullable: false),
                    Payment = table.Column<double>(nullable: false),
                    ProjectStatusDescription = table.Column<string>(nullable: true),
                    Completion = table.Column<double>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    BidAndTenderId = table.Column<int>(nullable: false),
                    CcompletionCertificateName = table.Column<string>(nullable: true),
                    CcompletionCertificatePath = table.Column<string>(nullable: true),
                    CcompletionCertificateFullPath = table.Column<string>(nullable: true),
                    CcompletionCertificateType = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    GrossAmount = table.Column<decimal>(nullable: false),
                    NetAmount = table.Column<decimal>(nullable: false),
                    PaymentStatus = table.Column<int>(nullable: false),
                    WorkflowToken = table.Column<string>(nullable: true),
                    ApprovalStatusId = table.Column<int>(nullable: false),
                    ProposedBy = table.Column<int>(nullable: false),
                    BankGl = table.Column<int>(nullable: false),
                    SubGl = table.Column<int>(nullable: false),
                    Taxes = table.Column<string>(nullable: true),
                    CompletionDate = table.Column<DateTime>(nullable: true),
                    EntryDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    InvoiceGenerated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_paymentterms", x => x.PaymentTermId);
                    table.ForeignKey(
                        name: "FK_cor_paymentterms_cor_bid_and_tender_BidAndTenderId",
                        column: x => x.BidAndTenderId,
                        principalTable: "cor_bid_and_tender",
                        principalColumn: "BidAndTenderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cor_supplier",
                columns: table => new
                {
                    SupplierId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierTypeId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Passport = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(maxLength: 550, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    PhoneNo = table.Column<string>(maxLength: 50, nullable: false),
                    RegistrationNo = table.Column<string>(maxLength: 50, nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    ApprovalStatusId = table.Column<int>(nullable: false),
                    Website = table.Column<string>(nullable: true),
                    TaxIDorVATID = table.Column<string>(nullable: true),
                    PostalAddress = table.Column<string>(nullable: true),
                    SupplierNumber = table.Column<string>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    HaveWorkPrintPermit = table.Column<bool>(nullable: false),
                    Particulars = table.Column<int>(nullable: false),
                    WorkflowToken = table.Column<string>(nullable: true),
                    cor_suppliertypeSupplierTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_supplier", x => x.SupplierId);
                    table.ForeignKey(
                        name: "FK_cor_supplier_cor_suppliertype_cor_suppliertypeSupplierTypeId",
                        column: x => x.cor_suppliertypeSupplierTypeId,
                        principalTable: "cor_suppliertype",
                        principalColumn: "SupplierTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purch_prndetails",
                columns: table => new
                {
                    PRNDetailsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    SubTotal = table.Column<decimal>(nullable: false),
                    PurchaseReqNoteId = table.Column<int>(nullable: false),
                    SuggestedSupplierId = table.Column<string>(nullable: true),
                    IsBudgeted = table.Column<bool>(nullable: true),
                    LPONumber = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    purch_requisitionnotePurchaseReqNoteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purch_prndetails", x => x.PRNDetailsId);
                    table.ForeignKey(
                        name: "FK_purch_prndetails_purch_requisitionnote_purch_requisitionnotePurchaseReqNoteId",
                        column: x => x.purch_requisitionnotePurchaseReqNoteId,
                        principalTable: "purch_requisitionnote",
                        principalColumn: "PurchaseReqNoteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cor_supplierauthorization",
                columns: table => new
                {
                    SupplierAuthorizationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Address = table.Column<string>(maxLength: 550, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    PhoneNo = table.Column<string>(maxLength: 50, nullable: false),
                    Signature = table.Column<byte[]>(type: "image", nullable: true),
                    cor_supplierSupplierId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_supplierauthorization", x => x.SupplierAuthorizationId);
                    table.ForeignKey(
                        name: "FK_cor_supplierauthorization_cor_supplier_cor_supplierSupplierId",
                        column: x => x.cor_supplierSupplierId,
                        principalTable: "cor_supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cor_supplierbusinessowner",
                columns: table => new
                {
                    SupplierBusinessOwnerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Address = table.Column<string>(maxLength: 550, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    PhoneNo = table.Column<string>(maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    Signature = table.Column<byte[]>(type: "image", nullable: true),
                    cor_supplierSupplierId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_supplierbusinessowner", x => x.SupplierBusinessOwnerId);
                    table.ForeignKey(
                        name: "FK_cor_supplierbusinessowner_cor_supplier_cor_supplierSupplierId",
                        column: x => x.cor_supplierSupplierId,
                        principalTable: "cor_supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cor_supplierdocument",
                columns: table => new
                {
                    SupplierDocumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false),
                    Document = table.Column<byte[]>(type: "image", nullable: true),
                    FileType = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    cor_supplierSupplierId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_supplierdocument", x => x.SupplierDocumentId);
                    table.ForeignKey(
                        name: "FK_cor_supplierdocument_cor_supplier_cor_supplierSupplierId",
                        column: x => x.cor_supplierSupplierId,
                        principalTable: "cor_supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cor_topsupplier",
                columns: table => new
                {
                    TopSupplierId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SupplierId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Address = table.Column<string>(maxLength: 550, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    PhoneNo = table.Column<string>(maxLength: 50, nullable: false),
                    ContactPerson = table.Column<string>(maxLength: 50, nullable: true),
                    NoOfStaff = table.Column<int>(nullable: true),
                    IncorporationDate = table.Column<DateTime>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    cor_supplierSupplierId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cor_topsupplier", x => x.TopSupplierId);
                    table.ForeignKey(
                        name: "FK_cor_topsupplier_cor_supplier_cor_supplierSupplierId",
                        column: x => x.cor_supplierSupplierId,
                        principalTable: "cor_supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purch_plpo",
                columns: table => new
                {
                    PLPOId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Address = table.Column<string>(maxLength: 550, nullable: false),
                    SupplierIds = table.Column<string>(nullable: true),
                    Tax = table.Column<decimal>(type: "money", nullable: false),
                    Total = table.Column<decimal>(type: "money", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "date", nullable: false),
                    LPONumber = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    ApprovalStatusId = table.Column<int>(nullable: false),
                    SupplierNumber = table.Column<string>(nullable: true),
                    SupplierAddress = table.Column<string>(nullable: true),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    GrossAmount = table.Column<decimal>(nullable: false),
                    AmountPayable = table.Column<decimal>(nullable: false),
                    JobStatus = table.Column<int>(nullable: false),
                    BidComplete = table.Column<bool>(nullable: false),
                    BidAndTenderId = table.Column<int>(nullable: false),
                    WinnerSupplierId = table.Column<int>(nullable: false),
                    WorkflowToken = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    PurchaseReqNoteId = table.Column<int>(nullable: false),
                    Taxes = table.Column<string>(nullable: true),
                    DebitGl = table.Column<int>(nullable: false),
                    ServiceTerm = table.Column<int>(nullable: false),
                    cor_supplierSupplierId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purch_plpo", x => x.PLPOId);
                    table.ForeignKey(
                        name: "FK_purch_plpo_cor_supplier_cor_supplierSupplierId",
                        column: x => x.cor_supplierSupplierId,
                        principalTable: "cor_supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purch_plpodetails",
                columns: table => new
                {
                    PLPODetailsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    SNo = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: false),
                    NoOfItems = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(type: "money", nullable: false),
                    SubTotal = table.Column<decimal>(type: "money", nullable: false),
                    PLPOId = table.Column<int>(nullable: false),
                    purch_plpoPLPOId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purch_plpodetails", x => x.PLPODetailsId);
                    table.ForeignKey(
                        name: "FK_purch_plpodetails_purch_plpo_purch_plpoPLPOId",
                        column: x => x.purch_plpoPLPOId,
                        principalTable: "purch_plpo",
                        principalColumn: "PLPOId",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_cor_paymentterms_BidAndTenderId",
                table: "cor_paymentterms",
                column: "BidAndTenderId");

            migrationBuilder.CreateIndex(
                name: "IX_cor_supplier_cor_suppliertypeSupplierTypeId",
                table: "cor_supplier",
                column: "cor_suppliertypeSupplierTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_cor_supplierauthorization_cor_supplierSupplierId",
                table: "cor_supplierauthorization",
                column: "cor_supplierSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_cor_supplierbusinessowner_cor_supplierSupplierId",
                table: "cor_supplierbusinessowner",
                column: "cor_supplierSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_cor_supplierdocument_cor_supplierSupplierId",
                table: "cor_supplierdocument",
                column: "cor_supplierSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_cor_topsupplier_cor_supplierSupplierId",
                table: "cor_topsupplier",
                column: "cor_supplierSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_purch_plpo_cor_supplierSupplierId",
                table: "purch_plpo",
                column: "cor_supplierSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_purch_plpodetails_purch_plpoPLPOId",
                table: "purch_plpodetails",
                column: "purch_plpoPLPOId");

            migrationBuilder.CreateIndex(
                name: "IX_purch_prndetails_purch_requisitionnotePurchaseReqNoteId",
                table: "purch_prndetails",
                column: "purch_requisitionnotePurchaseReqNoteId");
        }

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
                name: "ConfirmEmailCodes");

            migrationBuilder.DropTable(
                name: "cor_approvaldetail");

            migrationBuilder.DropTable(
                name: "cor_bankaccountdetail");

            migrationBuilder.DropTable(
                name: "cor_financialdetail");

            migrationBuilder.DropTable(
                name: "cor_identification");

            migrationBuilder.DropTable(
                name: "cor_paymentterms");

            migrationBuilder.DropTable(
                name: "cor_serviceterms");

            migrationBuilder.DropTable(
                name: "cor_supplierauthorization");

            migrationBuilder.DropTable(
                name: "cor_supplierbusinessowner");

            migrationBuilder.DropTable(
                name: "cor_supplierdocument");

            migrationBuilder.DropTable(
                name: "cor_taxsetup");

            migrationBuilder.DropTable(
                name: "cor_topclient");

            migrationBuilder.DropTable(
                name: "cor_topsupplier");

            migrationBuilder.DropTable(
                name: "inv_invoice");

            migrationBuilder.DropTable(
                name: "purch_invoice");

            migrationBuilder.DropTable(
                name: "purch_plpodetails");

            migrationBuilder.DropTable(
                name: "purch_prndetails");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "cor_bid_and_tender");

            migrationBuilder.DropTable(
                name: "purch_plpo");

            migrationBuilder.DropTable(
                name: "purch_requisitionnote");

            migrationBuilder.DropTable(
                name: "cor_supplier");

            migrationBuilder.DropTable(
                name: "cor_suppliertype");
        }
    }
}
