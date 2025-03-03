using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineClothing.Models;

public partial class ClothingShopPrn222G2Context : DbContext
{
    public ClothingShopPrn222G2Context()
    {
    }

    public ClothingShopPrn222G2Context(DbContextOptions<ClothingShopPrn222G2Context> options)
        : base(options)
    {
    }
    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartDetail> CartDetails { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductStatus> ProductStatuses { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportStatus> ReportStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGender> UserGenders { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserStatus> UserStatuses { get; set; }

    public virtual DbSet<UserVoucher> UserVouchers { get; set; }

    public virtual DbSet<Userinfo> Userinfos { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<VoucherStatus> VoucherStatuses { get; set; }

    public virtual DbSet<VoucherType> VoucherTypes { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DbConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cart__3213E83F4B063680");

            entity.ToTable("cart");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.TotalAmount)
                .HasDefaultValue(0)
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__cart__user_id__4F7CD00D");
        });

        modelBuilder.Entity<CartDetail>(entity =>
        {
            entity.HasKey(e => new { e.CartId, e.ProductId }).HasName("PK__cart_det__6A850DF80AE697FB");

            entity.ToTable("cart_detail");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(0)
                .HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasDefaultValue(0)
                .HasColumnName("total_price");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__cart_deta__cart___5535A963");

            entity.HasOne(d => d.Product).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__cart_deta__produ__5629CD9C");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__category__3213E83F110F6E97");

            entity.ToTable("category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__feedback__3213E83F5706FA01");

            entity.ToTable("feedback");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasMaxLength(511)
                .HasColumnName("comment");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__feedback__order___0F624AF8");

            entity.HasOne(d => d.Product).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__feedback__produc__0E6E26BF");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__feedback__user_i__0D7A0286");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__image__3213E83F95F10033");

            entity.ToTable("image");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("url");

            entity.HasOne(d => d.Product).WithMany(p => p.Images)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__image__product_i__02FC7413");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order__3213E83F37D9EE2A");

            entity.ToTable("order");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(127)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("full_name");
            entity.Property(e => e.Note)
                .HasMaxLength(511)
                .HasColumnName("note");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(25)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("phone_number");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderCustomers)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__order__customer___160F4887");

            entity.HasOne(d => d.Seller).WithMany(p => p.OrderSellers)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("FK__order__seller_id__17036CC0");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("FK__order__status__07C12930");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Orders)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK__order__voucher_i__0C85DE4D");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order_de__3213E83F595A1016");

            entity.ToTable("order_details");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalPrice).HasColumnName("total_price");
            entity.Property(e => e.UnitPrice).HasColumnName("unit_price");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__order_det__order__06CD04F7");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__order_det__produ__05D8E0BE");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order_st__3213E83F82C13F96");

            entity.ToTable("order_status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__product__3213E83F4C4F68DF");

            entity.ToTable("product");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.ThumbnailUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("thumbnail_url");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__product__categor__03F0984C");

            entity.HasOne(d => d.Seller).WithMany(p => p.Products)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("FK__product__seller___04E4BC85");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("FK__product__status__08B54D69");
        });

        modelBuilder.Entity<ProductStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__product___3213E83FE0E3B2E6");

            entity.ToTable("product_status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__report__3213E83F7B63014C");

            entity.ToTable("report");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Reason)
                .HasMaxLength(511)
                .HasColumnName("reason");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__report__product___114A936A");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("FK__report__status__123EB7A3");

            entity.HasOne(d => d.User).WithMany(p => p.Reports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__report__user_id__10566F31");
        });

        modelBuilder.Entity<ReportStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__report_s__3213E83FFBE1B384");

            entity.ToTable("report_status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__roles__3213E83F15B0D137");

            entity.ToTable("roles");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user__3213E83F666FA092");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("password");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasMaxLength(64)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("user_name");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("FK__user__status__09A971A2");
        });

        modelBuilder.Entity<UserGender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_gen__3213E83FB6F13C1A");

            entity.ToTable("user_gender");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("name");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_rol__3213E83F701AE510");

            entity.ToTable("user_roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("role_id");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__user_role__role___151B244E");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__user_role__user___02084FDA");
        });

        modelBuilder.Entity<UserStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_sta__3213E83FBAF279E6");

            entity.ToTable("user_status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<UserVoucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_vou__3213E83F06D0682F");

            entity.ToTable("user_voucher");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserVouchers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_vouc__user___17F790F9");

            entity.HasOne(d => d.Voucher).WithMany(p => p.UserVouchers)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK__user_vouc__vouch__18EBB532");
        });

        modelBuilder.Entity<Userinfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__userinfo__3213E83F0F859074");

            entity.ToTable("userinfo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("avatar_url");
            entity.Property(e => e.DateOfBirth)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("date_of_birth");
            entity.Property(e => e.FullName)
                .HasMaxLength(127)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("gender");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("phone_number");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");

            entity.HasOne(d => d.GenderNavigation).WithMany(p => p.Userinfos)
                .HasForeignKey(d => d.Gender)
                .HasConstraintName("FK__userinfo__gender__01142BA1");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Userinfo)
                .HasForeignKey<Userinfo>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__userinfo__id__00200768");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__voucher__3213E83F8F129B09");

            entity.ToTable("voucher");

            entity.HasIndex(e => e.Code, "UQ__voucher__357D4CF91212849A").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.Value)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("value");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("FK__voucher__status__0B91BA14");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Vouchers)
                .HasForeignKey(d => d.Type)
                .HasConstraintName("FK__voucher__type__0A9D95DB");
        });

        modelBuilder.Entity<VoucherStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__voucher___3213E83F0CC46EC9");

            entity.ToTable("voucher_status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<VoucherType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__voucher___3213E83F8EDEB1DF");

            entity.ToTable("voucher_type");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__wishlist__3213E83FD2F608FD");

            entity.ToTable("wishlist");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("create_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__wishlist__produc__14270015");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__wishlist__user_i__1332DBDC");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
