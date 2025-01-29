using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workly.Domain.Entities;

namespace Workly.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Tablo adı
            builder.ToTable("User");

            // Primary Key
            builder.HasKey(u => u.Id);

            // Email alanı
            builder.HasIndex(u => u.Email)
                .IsUnique();  // Email alanını unique yap

            builder.Property(u => u.Email)
                .IsRequired() // Email alanını zorunlu yap
                .HasMaxLength(100); // Maksimum uzunluğu 100 karakter olarak belirle

            // PasswordHash alanı
            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            // Name alanı
            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            // PhoneNumber alanı (opsiyonel)
            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            // IsActive alanı
            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(false);
            // LastLoginAt alanı (opsiyonel)
            builder.Property(u => u.LastLoginAt);

        }
    }
}
