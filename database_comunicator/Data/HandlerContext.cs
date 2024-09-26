using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using database_comunicator.Models;
using System.Security.Claims;

namespace database_comunicator.Data;

public partial class HandlerContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HandlerContext(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public HandlerContext(DbContextOptions<HandlerContext> options, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual DbSet<ActionLog> ActionLogs { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<AvailabilityStatus> AvailabilityStatuses { get; set; }

    public virtual DbSet<CalculatedCreditNotePrice> CalculatedCreditNotePrices { get; set; }

    public virtual DbSet<CalculatedPrice> CalculatedPrices { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<CreditNote> CreditNotes { get; set; }
    public virtual DbSet<CreditNoteItem> CreditNoteItems { get; set; }

    public virtual DbSet<CurrencyName> CurrencyNames { get; set; }

    public virtual DbSet<CurrencyValue> CurrencyValues { get; set; }

    public virtual DbSet<CurrencyValueOffer> CurrencyValueOffers { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<DeliveryCompany> DeliveryCompanies { get; set; }

    public virtual DbSet<DeliveryStatus> DeliveryStatuses { get; set; }

    public virtual DbSet<Ean> Eans { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemOwner> ItemOwners { get; set; }

    public virtual DbSet<LogType> LogTypes { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<ObjectType> ObjectTypes { get; set; }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<OfferItem> OfferItems { get; set; }

    public virtual DbSet<OfferStatus> OfferStatuses { get; set; }

    public virtual DbSet<OrgUser> OrgUsers { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<OutsideItem> OutsideItems { get; set; }

    public virtual DbSet<OutsideItemOffer> OutsideItemOffers { get; set; }

    public virtual DbSet<OwnedItem> OwnedItems { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<Proforma> Proformas { get; set; }

    public virtual DbSet<ProformaFutureItem> ProformaFutureItems { get; set; }

    public virtual DbSet<ProformaOwnedItem> ProformaOwnedItems { get; set; }

    public virtual DbSet<PurchasePrice> PurchasePrices { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestStatus> RequestStatuses { get; set; }

    public virtual DbSet<SellingPrice> SellingPrices { get; set; }

    public virtual DbSet<SoloUser> SoloUsers { get; set; }

    public virtual DbSet<Taxis> Taxes { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Waybill> Waybills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbName = _httpContextAccessor?.HttpContext?.Request.Path.ToString()
            .Split('/')[1];
        dbName ??= "template";
        var defPath = _configuration["ConnectionStrings:flexible"];
        var connectionString = defPath?.Replace("db_name", dbName);
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("Action_Log_pk");

            entity.ToTable("Action_Log");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.LogDate)
                .HasColumnType("date")
                .HasColumnName("log_date");
            entity.Property(e => e.LogDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("log_description");
            entity.Property(e => e.LogTypeId).HasColumnName("log_type_id");
            entity.Property(e => e.UsersId).HasColumnName("users_id");

            entity.HasOne(d => d.LogType).WithMany(p => p.ActionLogs)
                .HasForeignKey(d => d.LogTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Log_Log_Type_relation");

            entity.HasOne(d => d.Users).WithMany(p => p.ActionLogs)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Log_User_relation");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("App_User_pk");

            entity.ToTable("App_User");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Email)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.OrgUserId).HasColumnName("org_user_id");
            entity.Property(e => e.PassHash)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("pass_hash");
            entity.Property(e => e.PassSalt)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("pass_salt");
            entity.Property(e => e.SoloUserId).HasColumnName("solo_user_id");
            entity.Property(e => e.Surname)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("surname");
            entity.Property(e => e.Username)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.OrgUser).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.OrgUserId)
                .HasConstraintName("User_Org_User_relation");

            entity.HasOne(d => d.SoloUser).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.SoloUserId)
                .HasConstraintName("User_Solo_User_relation");

            entity.HasMany(d => d.Clients).WithMany(p => p.AppUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "UserClient",
                    r => r.HasOne<Organization>().WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("Client_User_relation"),
                    l => l.HasOne<AppUser>().WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("User_Client_relation"),
                    j =>
                    {
                        j.HasKey("UsersId", "OrganizationId").HasName("User_client_pk");
                        j.ToTable("User_client");
                        j.IndexerProperty<int>("UsersId").HasColumnName("users_id");
                        j.IndexerProperty<int>("OrganizationId").HasColumnName("organization_id");
                    });
        });

        modelBuilder.Entity<AvailabilityStatus>(entity =>
        {
            entity.HasKey(e => e.AvailabilityStatusId).HasName("Availability_Status_pk");

            entity.ToTable("Availability_Status");

            entity.Property(e => e.AvailabilityStatusId).HasColumnName("availability_status_id");
            entity.Property(e => e.DaysForRealization).HasColumnName("days_for_realization");
            entity.Property(e => e.StatusName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<CalculatedCreditNotePrice>(entity =>
        {
            entity.HasKey(e => new { e.CurrencyName, e.UpdateDate, e.CreditItemId }).HasName("Calculated_Credit_note_Price_pk");

            entity.ToTable("Calculated_Credit_note_Price");

            entity.Property(e => e.CurrencyName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("currency_name");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.CreditItemId).HasColumnName("credit_item_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.CreditNoteItem).WithMany(p => p.CalculatedCreditNotePrices)
                .HasForeignKey(d => d.CreditItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Calculated_Credit_note_Price_Credit_note_Items");

            entity.HasOne(d => d.CurrencyValue).WithMany(p => p.CalculatedCreditNotePrices)
                .HasForeignKey(d => new { d.UpdateDate, d.CurrencyName })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Calculated_Credit_note_Price_Currency_Value");
        });

        modelBuilder.Entity<CalculatedPrice>(entity =>
        {
            entity.HasKey(e => new { e.PurchasePriceId, e.UpdateDate, e.CurrencyName }).HasName("Calculated_Price_pk");

            entity.ToTable("Calculated_Price");

            entity.Property(e => e.PurchasePriceId).HasColumnName("purchase_price_id");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.CurrencyName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("currency_name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("price");

            entity.HasOne(d => d.PurchasePrice).WithMany(p => p.CalculatedPrices)
                .HasForeignKey(d => d.PurchasePriceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Calculated_Price_Purchase_Price_relation");

            entity.HasOne(d => d.CurrencyValue).WithMany(p => p.CalculatedPrices)
                .HasForeignKey(d => new { d.UpdateDate, d.CurrencyName })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Calculated_Price_Currency_Value_realtion");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("Country_pk");

            entity.ToTable("Country");

            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CountryName)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("country_name");
        });

        modelBuilder.Entity<CreditNote>(entity =>
        {
            entity.HasKey(e => e.IdCreditNote).HasName("Credit_note_pk");

            entity.ToTable("Credit_note");

            entity.Property(e => e.IdCreditNote).HasColumnName("id_credit_note");
            entity.Property(e => e.CreditFilePath)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("credit_file_path");
            entity.Property(e => e.CreditNoteDate)
                .HasColumnType("date")
                .HasColumnName("credit_note_date");
            entity.Property(e => e.CreditNoteNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("credit_note_number");
            entity.Property(e => e.InSystem).HasColumnName("in_system");
            entity.Property(e => e.IsPaid).HasColumnName("is_paid");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("note");
            entity.Property(e => e.IdUser).HasColumnName("id_user");

            entity.HasOne(d => d.Invoice).WithMany(p => p.CreditNotes)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Credit_note_Invoice_relation");
            entity.HasOne(d => d.User).WithMany(p => p.CreditNotes)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Credit_note_App_User_relation");
        });

        modelBuilder.Entity<CreditNoteItem>(entity =>
        {
            entity.HasKey(e => e.CreditItemId).HasName("Credit_note_Items_pk");

            entity.ToTable("Credit_note_Items");

            entity.Property(e => e.CreditItemId)
                .HasColumnName("credit_item_id");
            entity.Property(e => e.CreditNoteId)
                .HasColumnName("credit_note_id");
            entity.Property(e => e.NewPrice)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("new_price");
            entity.Property(e => e.PurchasePriceId).HasColumnName("purchase_price_id");
            entity.Property(e => e.Qty).HasColumnName("qty");

            entity.HasOne(d => d.CreditNote).WithMany(p => p.CreditNoteItems)
                .HasForeignKey(d => d.CreditNoteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Credit_note_Items_Credit_note_relation");

            entity.HasOne(d => d.PurchasePrice).WithMany(p => p.CreditNoteItems)
                .HasForeignKey(d => d.PurchasePriceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Credit_note_Items_Purchase_Price_relation");
        });

        modelBuilder.Entity<CurrencyName>(entity =>
        {
            entity.HasKey(e => e.Curenncy).HasName("Currency_Name_pk");

            entity.ToTable("Currency_Name");

            entity.Property(e => e.Curenncy)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("curenncy");
        });

        modelBuilder.Entity<CurrencyValue>(entity =>
        {
            entity.HasKey(e => new { e.UpdateDate, e.CurrencyName }).HasName("Currency_Value_pk");

            entity.ToTable("Currency_Value");

            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.CurrencyName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("currency_name");
            entity.Property(e => e.CurrencyValue1)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("currency_value");

            entity.HasOne(d => d.CurrencyNameNavigation).WithMany(p => p.CurrencyValues)
                .HasForeignKey(d => d.CurrencyName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Currency_Value_Currency_Name_relation");
        });

        modelBuilder.Entity<CurrencyValueOffer>(entity =>
        {
            entity.HasKey(e => new { e.OfferId, e.CurrancyName }).HasName("Currency_Value_Offer_pk");

            entity.ToTable("Currency_Value_Offer");

            entity.Property(e => e.OfferId).HasColumnName("offer_id");
            entity.Property(e => e.CurrancyName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("currancy_name");
            entity.Property(e => e.CurencyDate)
                .HasColumnType("datetime")
                .HasColumnName("curency_date");

            entity.HasOne(d => d.Offer).WithMany(p => p.CurrencyValueOffers)
                .HasForeignKey(d => d.OfferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Currency_Value_Offer_Offer_relation");

            entity.HasOne(d => d.Cur).WithMany(p => p.CurrencyValueOffers)
                .HasForeignKey(d => new { d.CurencyDate, d.CurrancyName })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Currency_Value_Offer_Currency_Value_relation");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("Delivery_pk");

            entity.ToTable("Delivery");

            entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");
            entity.Property(e => e.DeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("delivery_date");
            entity.Property(e => e.DeliveryStatusId).HasColumnName("delivery_status_id");
            entity.Property(e => e.EstimatedDeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("estimated_delivery_date");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.ProformaId).HasColumnName("proforma_id");

            entity.HasOne(d => d.DeliveryStatus).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.DeliveryStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Delivery_Delivery_Status_relation");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("Delivery_Invoice_relation");

            entity.HasOne(d => d.Proforma).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.ProformaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Delivery_Proforma_relation");

            entity.HasMany(d => d.Notes).WithMany(p => p.Deliveries)
                .UsingEntity<Dictionary<string, object>>(
                    "NotesDelivery",
                    r => r.HasOne<Note>().WithMany()
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("Note_Delivery_Note_relation"),
                    l => l.HasOne<Delivery>().WithMany()
                        .HasForeignKey("DeliveryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("Note_Delivery_Delivery_relation"),
                    j =>
                    {
                        j.HasKey("DeliveryId", "NoteId").HasName("Notes_Delivery_pk");
                        j.ToTable("Notes_Delivery");
                        j.IndexerProperty<int>("DeliveryId").HasColumnName("delivery_id");
                        j.IndexerProperty<int>("NoteId").HasColumnName("note_id");
                    });
        });

        modelBuilder.Entity<DeliveryCompany>(entity =>
        {
            entity.HasKey(e => e.DeliveryCompanyId).HasName("Delivery_company_pk");

            entity.ToTable("Delivery_company");

            entity.Property(e => e.DeliveryCompanyId).HasColumnName("delivery_company_id");
            entity.Property(e => e.DeliveryCompanyName)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("delivery_company_name");
        });

        modelBuilder.Entity<DeliveryStatus>(entity =>
        {
            entity.HasKey(e => e.DeliveryStatusId).HasName("Delivery_Status_pk");

            entity.ToTable("Delivery_Status");

            entity.Property(e => e.DeliveryStatusId).HasColumnName("delivery_status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Ean>(entity =>
        {
            entity.HasKey(e => e.EanValue).HasName("EAN_pk");

            entity.ToTable("EAN");

            entity.Property(e => e.EanValue)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ean");
            entity.Property(e => e.ItemId).HasColumnName("item_id");

            entity.HasOne(d => d.Item).WithMany(p => p.Eans)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EAN_Item_relation");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("Invoice_pk");

            entity.ToTable("Invoice");

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.Buyer).HasColumnName("buyer");
            entity.Property(e => e.CurrencyName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("currency_name");
            entity.Property(e => e.CurrencyValueDate)
                .HasColumnType("datetime")
                .HasColumnName("currency_value_date");
            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasColumnName("due_date");
            entity.Property(e => e.InSystem).HasColumnName("in_system");
            entity.Property(e => e.InvoiceDate)
                .HasColumnType("datetime")
                .HasColumnName("invoice_date");
            entity.Property(e => e.InvoiceFilePath)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("invoice_file_path");
            entity.Property(e => e.InvoiceNumber)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("invoice_number");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("note");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.PaymentsStatusId).HasColumnName("payments_status_Id");
            entity.Property(e => e.Seller).HasColumnName("seller");
            entity.Property(e => e.Taxes).HasColumnName("taxes");
            entity.Property(e => e.TransportCost)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("transport_cost");

            entity.HasOne(d => d.BuyerNavigation).WithMany(p => p.InvoiceBuyerNavigations)
                .HasForeignKey(d => d.Buyer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("buyer_invoice_relation");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Invoice_Payment_Method_relation");

            entity.HasOne(d => d.PaymentsStatus).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PaymentsStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Invoice_Payments_Status_relation");

            entity.HasOne(d => d.SellerNavigation).WithMany(p => p.InvoiceSellerNavigations)
                .HasForeignKey(d => d.Seller)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Seller_Organization_relation");

            entity.HasOne(d => d.TaxesNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.Taxes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Invoice_Taxes_relation");

            entity.HasOne(d => d.Currency).WithMany(p => p.Invoices)
                .HasForeignKey(d => new { d.CurrencyValueDate, d.CurrencyName })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Invoice_Currency_Value_relation");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("Item_pk");

            entity.ToTable("Item");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("item_description");
            entity.Property(e => e.ItemName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("item_name");
            entity.Property(e => e.PartNumber)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("part_number");
        });

        modelBuilder.Entity<ItemOwner>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.InvoiceId, e.OwnedItemId }).HasName("Item_owner_pk");

            entity.ToTable("Item_owner");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.OwnedItemId).HasColumnName("owned_item_id");
            entity.Property(e => e.Qty).HasColumnName("qty");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.ItemOwners)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Item_owner_User_relation");

            entity.HasOne(d => d.OwnedItem).WithMany(p => p.ItemOwners)
                .HasForeignKey(d => new { d.OwnedItemId, d.InvoiceId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Item_owner_Owned_Item_relation");
        });

        modelBuilder.Entity<LogType>(entity =>
        {
            entity.HasKey(e => e.LogTypeId).HasName("Log_Type_pk");

            entity.ToTable("Log_Type");

            entity.Property(e => e.LogTypeId).HasColumnName("log_type_id");
            entity.Property(e => e.LogTypeName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("log_type_name");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.NoteId).HasName("Note_pk");

            entity.ToTable("Note");

            entity.Property(e => e.NoteId).HasColumnName("note_id");
            entity.Property(e => e.NoteDate)
                .HasColumnType("datetime")
                .HasColumnName("note_date");
            entity.Property(e => e.NoteDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("note_description");
            entity.Property(e => e.UsersId).HasColumnName("users_id");

            entity.HasOne(d => d.Users).WithMany(p => p.Notes)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Note_User_relation");
        });

        modelBuilder.Entity<ObjectType>(entity =>
        {
            entity.HasKey(e => e.ObjectTypeId).HasName("Object_type_pk");

            entity.ToTable("Object_type");

            entity.Property(e => e.ObjectTypeId).HasColumnName("object_type_id");
            entity.Property(e => e.ObjectTypeName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("object_type_name");
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.OfferId).HasName("Offer_pk");

            entity.ToTable("Offer");

            entity.Property(e => e.OfferId).HasColumnName("offer_id");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.ModificationDate).HasColumnName("modification_date");
            entity.Property(e => e.OfferName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("offer_name");
            entity.Property(e => e.OfferStatusId).HasColumnName("offer_status_id");
            entity.Property(e => e.OrganizationsId).HasColumnName("organizations_id");
            entity.Property(e => e.PathToFile)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("path_to_file");

            entity.HasOne(d => d.OfferStatus).WithMany(p => p.Offers)
                .HasForeignKey(d => d.OfferStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Offer_Offer_status_relation");

            entity.HasOne(d => d.Organizations).WithMany(p => p.Offers)
                .HasForeignKey(d => d.OrganizationsId)
                .HasConstraintName("Offer_Organization_relation");
        });

        modelBuilder.Entity<OfferItem>(entity =>
        {
            entity.HasKey(e => e.OfferItemId).HasName("Offer_Item_pk");

            entity.ToTable("Offer_Item");

            entity.Property(e => e.OfferItemId)
                .HasColumnName("offer_item_id");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.OfferId).HasColumnName("offer_id");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.SellingPrice)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("selling_price");

            entity.HasOne(d => d.Offer).WithMany(p => p.OfferItems)
                .HasForeignKey(d => d.OfferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Offer_Items_relation");

            entity.HasOne(d => d.ItemOwner).WithMany(p => p.OfferItems)
                .HasForeignKey(d => new { d.IdUser, d.InvoiceId, d.ItemId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Offer_Item_Item_owner_relation");
        });

        modelBuilder.Entity<OfferStatus>(entity =>
        {
            entity.HasKey(e => e.OfferId).HasName("Offer_status_pk");

            entity.ToTable("Offer_status");

            entity.Property(e => e.OfferId).HasColumnName("offer_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<OrgUser>(entity =>
        {
            entity.HasKey(e => e.OrgUserId).HasName("Org_User_pk");

            entity.ToTable("Org_User");

            entity.Property(e => e.OrgUserId).HasColumnName("org_user_id");
            entity.Property(e => e.OrganizationsId).HasColumnName("organizations_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Organizations).WithMany(p => p.OrgUsers)
                .HasForeignKey(d => d.OrganizationsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Org_User_Organization_relation");

            entity.HasOne(d => d.Role).WithMany(p => p.OrgUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Org_User_Role_relation");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("Organization_pk");

            entity.ToTable("Organization");

            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.AvailabilityStatusId).HasColumnName("availability_status_id");
            entity.Property(e => e.City)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.CountryId).HasColumnName("country_id");
            entity.Property(e => e.CreditLimit).HasColumnName("credit_limit");
            entity.Property(e => e.Nip).HasColumnName("nip");
            entity.Property(e => e.OrgName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("org_name");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("postal_code");
            entity.Property(e => e.Street)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("street");

            entity.HasOne(d => d.AvailabilityStatus).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.AvailabilityStatusId)
                .HasConstraintName("Organization_Availability_Status_relation");

            entity.HasOne(d => d.Country).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Organization_Country_relation");
        });

        modelBuilder.Entity<OutsideItem>(entity =>
        {
            entity.HasKey(e => new { e.ItemId, e.OrganizationId }).HasName("Outside_Item_pk");

            entity.ToTable("Outside_Item");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.CurrencyName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("currency_name");
            entity.Property(e => e.PurchasePrice)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("purchase_price");
            entity.Property(e => e.Qty).HasColumnName("qty");

            entity.HasOne(d => d.CurrencyNameNavigation).WithMany(p => p.OutsideItems)
                .HasForeignKey(d => d.CurrencyName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Outside_Item_Currency_relation");

            entity.HasOne(d => d.Item).WithMany(p => p.OutsideItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Outside_Item_Item_relation");

            entity.HasOne(d => d.Organization).WithMany(p => p.OutsideItems)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Outside_Item_Organization_relation");
        });

        modelBuilder.Entity<OutsideItemOffer>(entity =>
        {
            entity.HasKey(e => new { e.OfferId, e.OrganizationId, e.OutsideItemId }).HasName("Outside_Item_Offer_pk");

            entity.ToTable("Outside_Item_Offer");

            entity.Property(e => e.OfferId).HasColumnName("offer_id");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.OutsideItemId).HasColumnName("outside_item_id");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.SellingPrice)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("selling_price");

            entity.HasOne(d => d.Offer).WithMany(p => p.OutsideItemOffers)
                .HasForeignKey(d => d.OfferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Outside_Item_Offer_Offer_relation");

            entity.HasOne(d => d.O).WithMany(p => p.OutsideItemOffers)
                .HasForeignKey(d => new { d.OutsideItemId, d.OrganizationId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Outside_Item_Offer_Outside_Item_relation");
        });

        modelBuilder.Entity<OwnedItem>(entity =>
        {
            entity.HasKey(e => new { e.OwnedItemId, e.InvoiceId }).HasName("Owned_Item_pk");

            entity.ToTable("Owned_Item");

            entity.Property(e => e.OwnedItemId).HasColumnName("owned_item_id");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");

            entity.HasOne(d => d.Invoice).WithMany(p => p.OwnedItems)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Owned_Item_Invoice_relation");

            entity.HasOne(d => d.OriginalItem).WithMany(p => p.OwnedItems)
                .HasForeignKey(d => d.OwnedItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Owned_Item_Item_relation");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("Payment_Method_pk");

            entity.ToTable("Payment_Method");

            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.MethodName)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasColumnName("method_name");
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.PaymentStatusId).HasName("Payment_Status_pk");

            entity.ToTable("Payment_Status");

            entity.Property(e => e.PaymentStatusId).HasColumnName("payment_status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Proforma>(entity =>
        {
            entity.HasKey(e => e.ProformaId).HasName("Proforma_pk");

            entity.ToTable("Proforma");

            entity.Property(e => e.ProformaId).HasColumnName("proforma_id");
            entity.Property(e => e.Buyer).HasColumnName("buyer");
            entity.Property(e => e.CurrencyName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("currency_name");
            entity.Property(e => e.CurrencyValueDate)
                .HasColumnType("datetime")
                .HasColumnName("currency_value_date");
            entity.Property(e => e.InSystem).HasColumnName("in_system");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("note");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.ProformaDate)
                .HasColumnType("datetime")
                .HasColumnName("proforma_date");
            entity.Property(e => e.ProformaFilePath)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("proforma_file_path");
            entity.Property(e => e.ProformaNumber)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("proforma_number");
            entity.Property(e => e.Seller).HasColumnName("seller");
            entity.Property(e => e.Taxes).HasColumnName("taxes");
            entity.Property(e => e.TransportCost)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("transport_cost");

            entity.HasOne(d => d.BuyerNavigation).WithMany(p => p.ProformaBuyerNavigations)
                .HasForeignKey(d => d.Buyer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("buyer_Proforma_relation");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Proformas)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("Proforma_Invoice_relation");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Proformas)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Proforma_Payment_Method_relation");

            entity.HasOne(d => d.SellerNavigation).WithMany(p => p.ProformaSellerNavigations)
                .HasForeignKey(d => d.Seller)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Seller_Proforma_relation");

            entity.HasOne(d => d.TaxesNavigation).WithMany(p => p.Proformas)
                .HasForeignKey(d => d.Taxes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Proforma_Taxes_relation");

            entity.HasOne(d => d.Currency).WithMany(p => p.Proformas)
                .HasForeignKey(d => new { d.CurrencyValueDate, d.CurrencyName })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Proforma_Currency_Value");
        });

        modelBuilder.Entity<ProformaFutureItem>(entity =>
        {
            entity.HasKey(e => e.ProformaFutureItemId).HasName("Proforma_Future_Item_pk");

            entity.ToTable("Proforma_Future_Item");

            entity.Property(e => e.ProformaFutureItemId)
                .HasColumnName("proforma_future_item_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ProformaId).HasColumnName("proforma_id");
            entity.Property(e => e.PurchasePrice)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("purchase_price");
            entity.Property(e => e.Qty).HasColumnName("qty");

            entity.HasOne(d => d.Item).WithMany(p => p.ProformaFutureItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Proforma_Future_Item_Item");

            entity.HasOne(d => d.Proforma).WithMany(p => p.ProformaFutureItems)
                .HasForeignKey(d => d.ProformaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Proforma_Future_Item_Proforma");
        });

        modelBuilder.Entity<ProformaOwnedItem>(entity =>
        {
            entity.HasKey(e => e.ProformaOwnedItemId).HasName("Proforma_Owned_Item_pk");

            entity.ToTable("Proforma_Owned_Item");

            entity.Property(e => e.ProformaOwnedItemId)
                .HasColumnName("proforma_owned_item_id");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.OwnedItemId).HasColumnName("owned_item_id");
            entity.Property(e => e.ProformaId).HasColumnName("proforma_id");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.SellingPrice)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("selling_price");

            entity.HasOne(d => d.Proforma).WithMany(p => p.ProformaOwnedItems)
                .HasForeignKey(d => d.ProformaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Proforma_Owned_Item_Proforma_relation");

            entity.HasOne(d => d.OwnedItem).WithMany(p => p.ProformaOwnedItems)
                .HasForeignKey(d => new { d.OwnedItemId, d.InvoiceId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Proforma_Owned_Item_Owned_Item_relation");
        });

        modelBuilder.Entity<PurchasePrice>(entity =>
        {
            entity.HasKey(e => e.PurchasePriceId).HasName("Purchase_Price_pk");

            entity.ToTable("Purchase_Price");

            entity.Property(e => e.PurchasePriceId).HasColumnName("purchase_price_id");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.OwnedItemId).HasColumnName("owned_item_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Qty).HasColumnName("qty");

            entity.HasOne(d => d.OwnedItem).WithMany(p => p.PurchasePrices)
                .HasForeignKey(d => new { d.OwnedItemId, d.InvoiceId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Purchase_Price_Owned_Item_relation");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("Request_pk");

            entity.ToTable("Request");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.IdUserCreator).HasColumnName("id_user_creator");
            entity.Property(e => e.IdUserReciver).HasColumnName("id_user_receiver");
            entity.Property(e => e.RequestStatusId).HasColumnName("request_status_id");
            entity.Property(e => e.ObjectTypeId).HasColumnName("object_type_id");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("note");
            entity.Property(e => e.FilePath)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("filePath");

            entity.HasOne(d => d.UserCreator).WithMany(p => p.CreatedRequest)
                .HasForeignKey(d => d.IdUserCreator)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Request_App_User_creator_relation");

            entity.HasOne(d => d.UserReciver).WithMany(p => p.Requests)
                .HasForeignKey(d => d.IdUserReciver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Request_App_User_relation");

            entity.HasOne(d => d.RequestStatus).WithMany(p => p.Requests)
                .HasForeignKey(d => d.RequestStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Request_Request_status_relation");

            entity.HasOne(d => d.ObjectType).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ObjectTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Request_Object_type_relation");
        });

        modelBuilder.Entity<RequestStatus>(entity =>
        {
            entity.HasKey(e => e.RequestStatusId).HasName("Request_status_pk");

            entity.ToTable("Request_status");

            entity.Property(e => e.RequestStatusId).HasColumnName("request_status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<SellingPrice>(entity =>
        {
            entity.HasKey(e => e.SellingPriceId).HasName("Selling_Price_pk");

            entity.ToTable("Selling_Price");

            entity.Property(e => e.SellingPriceId).HasColumnName("selling_price_id");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(20, 2)")
                .HasColumnName("price");
            entity.Property(e => e.PurchasePriceId).HasColumnName("purchase_price_id");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.SellInvoiceId).HasColumnName("sell_invoice_id");

            entity.HasOne(d => d.User).WithMany(p => p.SellingPrices)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Selling_Price_App_User_relation");

            entity.HasOne(d => d.PurchasePrice).WithMany(p => p.SellingPrices)
                .HasForeignKey(d => d.PurchasePriceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Selling_Price_Purchase_Price_relation");

            entity.HasOne(d => d.SellInvoice).WithMany(p => p.SellingPrices)
                .HasForeignKey(d => d.SellInvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Selling_Price_Invoice");
        });

        modelBuilder.Entity<SoloUser>(entity =>
        {
            entity.HasKey(e => e.SoloUserId).HasName("Solo_User_pk");

            entity.ToTable("Solo_User");

            entity.Property(e => e.SoloUserId).HasColumnName("solo_user_id");
            entity.Property(e => e.OrganizationsId).HasColumnName("organizations_id");

            entity.HasOne(d => d.Organizations).WithMany(p => p.SoloUsers)
                .HasForeignKey(d => d.OrganizationsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Solo_User_Organization_relation");
        });

        modelBuilder.Entity<Taxis>(entity =>
        {
            entity.HasKey(e => e.TaxesId).HasName("Taxes_pk");

            entity.Property(e => e.TaxesId).HasColumnName("taxes_id");
            entity.Property(e => e.TaxValue)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("tax_value");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("User_notification_pk");

            entity.ToTable("User_notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.Info)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("info");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.ObjectTypeId).HasColumnName("object_type_id");
            entity.Property(e => e.Referance)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("referance");
            entity.Property(e => e.UsersId).HasColumnName("users_id");

            entity.HasOne(d => d.ObjectType).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.ObjectTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Notification_Object_type_relation");

            entity.HasOne(d => d.Users).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Notification_User_relation");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("User_role_pk");

            entity.ToTable("User_role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Waybill>(entity =>
        {
            entity.HasKey(e => e.WaybillId).HasName("Waybill_pk");

            entity.ToTable("Waybill");

            entity.Property(e => e.WaybillId).HasColumnName("waybill_id");
            entity.Property(e => e.DeliveriesId).HasColumnName("deliveries_id");
            entity.Property(e => e.DeliveryCompanyId).HasColumnName("delivery_company_id");
            entity.Property(e => e.Waybill1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("waybill");

            entity.HasOne(d => d.Deliveries).WithMany(p => p.Waybills)
                .HasForeignKey(d => d.DeliveriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Waybill_Delivery_relation");

            entity.HasOne(d => d.DeliveryCompany).WithMany(p => p.Waybills)
                .HasForeignKey(d => d.DeliveryCompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Waybill_Delivery_company_relation");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
