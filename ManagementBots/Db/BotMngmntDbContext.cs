using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ManagementBots.Db
{
    public partial class BotMngmntDbContext : DbContext
    {
        public virtual DbSet<ActionHistory> ActionHistory { get; set; }
        public virtual DbSet<ActionType> ActionType { get; set; }
        public virtual DbSet<AttachmentFs> AttachmentFs { get; set; }
        public virtual DbSet<AttachmentTelegram> AttachmentTelegram { get; set; }
        public virtual DbSet<AttachmentType> AttachmentType { get; set; }
        public virtual DbSet<Bot> Bot { get; set; }
        public virtual DbSet<BotBlocked> BotBlocked { get; set; }
        public virtual DbSet<BotDeleted> BotDeleted { get; set; }
        public virtual DbSet<BotInfo> BotInfo { get; set; }
        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<Dns> Dns { get; set; }
        public virtual DbSet<Follower> Follower { get; set; }
        public virtual DbSet<HelpDesk> HelpDesk { get; set; }
        public virtual DbSet<HelpDeskAnswer> HelpDeskAnswer { get; set; }
        public virtual DbSet<HelpDeskAttachment> HelpDeskAttachment { get; set; }
        public virtual DbSet<HelpDeskInWork> HelpDeskInWork { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PaymentSystem> PaymentSystem { get; set; }
        public virtual DbSet<PaymentSystemConfig> PaymentSystemConfig { get; set; }
        public virtual DbSet<ProxyServer> ProxyServer { get; set; }
        public virtual DbSet<ReserveWebApp> ReserveWebApp { get; set; }
        public virtual DbSet<ReserveWebHookUrl> ReserveWebHookUrl { get; set; }
        public virtual DbSet<ServerWebApp> ServerWebApp { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<ServiceBotHistory> ServiceBotHistory { get; set; }
        public virtual DbSet<ServiceType> ServiceType { get; set; }
        public virtual DbSet<WebApp> WebApp { get; set; }
        public virtual DbSet<WebAppHistory> WebAppHistory { get; set; }
        public virtual DbSet<WebHookPort> WebHookPort { get; set; }
        public virtual DbSet<WebHookUrl> WebHookUrl { get; set; }
        public virtual DbSet<WebHookUrlHistory> WebHookUrlHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=DESKTOP-93GE9VD;Database=BotMngmntDb;Integrated Security = True; Trusted_Connection = True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionHistory>(entity =>
            {
                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.ActionType)
                    .WithMany(p => p.ActionHistory)
                    .HasForeignKey(d => d.ActionTypeId)
                    .HasConstraintName("FK_ActionHistory_ActionType");
            });

            modelBuilder.Entity<ActionType>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AttachmentFs>(entity =>
            {
                entity.HasIndex(e => e.GuId)
                    .HasName("UQ__Attachme__A2B66B0514E78B0A")
                    .IsUnique();

                entity.HasIndex(e => e.Id)
                    .HasName("UQ__Attachme__3214EC06D860A4AD")
                    .IsUnique();

                entity.Property(e => e.Caption)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.AttachmentType)
                    .WithMany(p => p.AttachmentFs)
                    .HasForeignKey(d => d.AttachmentTypeId)
                    .HasConstraintName("FK_AttachmentFs_AttachmentType");
            });

            modelBuilder.Entity<AttachmentTelegram>(entity =>
            {
                entity.Property(e => e.FileId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.AttachmentFs)
                    .WithMany(p => p.AttachmentTelegram)
                    .HasForeignKey(d => d.AttachmentFsId)
                    .HasConstraintName("FK_Attachment_AttachmentFs");

                entity.HasOne(d => d.BotInfo)
                    .WithMany(p => p.AttachmentTelegram)
                    .HasForeignKey(d => d.BotInfoId)
                    .HasConstraintName("FK_Attachment_BotInfo");
            });

            modelBuilder.Entity<AttachmentType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Bot>(entity =>
            {
                entity.Property(e => e.BotName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreateTimeStamp).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasMaxLength(1500)
                    .IsUnicode(false);

                entity.Property(e => e.Token)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.Bot)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_Bot_Follower");

                entity.HasOne(d => d.ProxyServer)
                    .WithMany(p => p.Bot)
                    .HasForeignKey(d => d.ProxyServeId)
                    .HasConstraintName("FK_Bot_ProxyServer");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Bot)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_Bot_ServiceList");

                entity.HasOne(d => d.WebApp)
                    .WithMany(p => p.Bot)
                    .HasForeignKey(d => d.WebAppId)
                    .HasConstraintName("FK_Bot_WebApp");

                entity.HasOne(d => d.WebHookUrl)
                    .WithMany(p => p.Bot)
                    .HasForeignKey(d => d.WebHookUrlId)
                    .HasConstraintName("FK_Bot_WebHookUrl");
            });

            modelBuilder.Entity<BotBlocked>(entity =>
            {
                entity.Property(e => e.BlockedTimeStamp).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.UnblockedTimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.Bot)
                    .WithMany(p => p.BotBlocked)
                    .HasForeignKey(d => d.BotId)
                    .HasConstraintName("FK_BotBlocked_Bot");
            });

            modelBuilder.Entity<BotDeleted>(entity =>
            {
                entity.Property(e => e.DeletedTimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.Bot)
                    .WithMany(p => p.BotDeleted)
                    .HasForeignKey(d => d.BotId)
                    .HasConstraintName("FK_BotDeleted_Bot");
            });

            modelBuilder.Entity<BotInfo>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.WebHookUrl)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.HasIndex(e => e.BotInfoId)
                    .HasName("IX_Configuration_BotInfoId")
                    .IsUnique();

                entity.Property(e => e.ManualFileId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PrivateGroupChatId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sslpath)
                    .HasColumnName("SSLPath")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.UserNameFaqFileId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.BotInfo)
                    .WithOne(p => p.Configuration)
                    .HasForeignKey<Configuration>(d => d.BotInfoId)
                    .HasConstraintName("FK_Configuration_BotInfo");
            });

            modelBuilder.Entity<Dns>(entity =>
            {
                entity.ToTable("DNS");

                entity.Property(e => e.Ip)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SslPathOnMainServer)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SslPathOnProxy)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<Follower>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .HasColumnName("FIrstName")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Telephone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<HelpDesk>(entity =>
            {
                entity.Property(e => e.Text)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.HasOne(d => d.BotInfo)
                    .WithMany(p => p.HelpDesk)
                    .HasForeignKey(d => d.BotInfoId)
                    .HasConstraintName("FK_HelpDesk_BotInfo");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.HelpDesk)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_HelpDesk_Follower");
            });

            modelBuilder.Entity<HelpDeskAnswer>(entity =>
            {
                entity.Property(e => e.ClosedTimestamp).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.HelpDeskAnswer)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_HelpDeskAnswer_Follower");

                entity.HasOne(d => d.HelpDesk)
                    .WithMany(p => p.HelpDeskAnswer)
                    .HasForeignKey(d => d.HelpDeskId)
                    .HasConstraintName("FK_HelpDeskAnswer_HelpDesk");
            });

            modelBuilder.Entity<HelpDeskAttachment>(entity =>
            {
                entity.HasKey(e => new { e.HelpDeskId, e.AttachmentFsId });

                entity.HasOne(d => d.AttachmentFs)
                    .WithMany(p => p.HelpDeskAttachment)
                    .HasForeignKey(d => d.AttachmentFsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HelpDeskAttachment_Attachment");

                entity.HasOne(d => d.HelpDesk)
                    .WithMany(p => p.HelpDeskAttachment)
                    .HasForeignKey(d => d.HelpDeskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HelpDeskAttachment_HelpDesk");
            });

            modelBuilder.Entity<HelpDeskInWork>(entity =>
            {
                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.HelpDeskInWork)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_HelpDeskInWork_Follower");

                entity.HasOne(d => d.HelpDesk)
                    .WithMany(p => p.HelpDeskInWork)
                    .HasForeignKey(d => d.HelpDeskId)
                    .HasConstraintName("FK_HelpDeskInWork_HelpDesk");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.AccountNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Comment)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreateTimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.PaymentSystem)
                    .WithMany(p => p.Invoice)
                    .HasForeignKey(d => d.PaymentSystemId)
                    .HasConstraintName("FK_Invoice_PaymentSystem");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.DateAdd).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("FK_Notification_Follower");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.CreateTimeStamp).HasColumnType("datetime");

                entity.Property(e => e.PaymentTimeStamp).HasColumnType("datetime");

                entity.Property(e => e.SenderAccountNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TxId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Payment)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("FK_Payment_Invoice");
            });

            modelBuilder.Entity<PaymentSystem>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PaymentSystemConfig>(entity =>
            {
                entity.Property(e => e.Login)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pass)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Server)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.PaymentSystem)
                    .WithMany(p => p.PaymentSystemConfig)
                    .HasForeignKey(d => d.PaymentSystemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentSystemConfig_PaymentSystem");
            });

            modelBuilder.Entity<ProxyServer>(entity =>
            {
                entity.Property(e => e.CertPath)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ip)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PassPhrase)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReserveWebApp>(entity =>
            {
                entity.HasKey(e => new { e.BotId, e.WebAppId });

                entity.HasIndex(e => e.BotId)
                    .HasName("IX_ReserveWebApp_BotId")
                    .IsUnique();

                entity.HasIndex(e => e.WebAppId)
                    .HasName("IX_ReserveWebApp_Id")
                    .IsUnique();

                entity.Property(e => e.TimeStampEnd).HasColumnType("datetime");

                entity.Property(e => e.TimeStampStart).HasColumnType("datetime");

                entity.HasOne(d => d.Bot)
                    .WithOne(p => p.ReserveWebApp)
                    .HasForeignKey<ReserveWebApp>(d => d.BotId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReserveWebApp_Bot");

                entity.HasOne(d => d.WebApp)
                    .WithOne(p => p.ReserveWebApp)
                    .HasForeignKey<ReserveWebApp>(d => d.WebAppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReserveWebApp_WebApp");
            });

            modelBuilder.Entity<ReserveWebHookUrl>(entity =>
            {
                entity.HasKey(e => new { e.BotId, e.WebHookUrlId });

                entity.HasIndex(e => e.BotId)
                    .HasName("IX_ReserveWebHookUrl_BotId")
                    .IsUnique();

                entity.HasIndex(e => e.WebHookUrlId)
                    .HasName("IX_ReserveWebHookUrl_Id")
                    .IsUnique();

                entity.Property(e => e.TimeStampEnd).HasColumnType("datetime");

                entity.Property(e => e.TimeStampStart).HasColumnType("datetime");

                entity.HasOne(d => d.Bot)
                    .WithOne(p => p.ReserveWebHookUrl)
                    .HasForeignKey<ReserveWebHookUrl>(d => d.BotId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReserveWebHookUrl_Bot");

                entity.HasOne(d => d.WebHookUrl)
                    .WithOne(p => p.ReserveWebHookUrl)
                    .HasForeignKey<ReserveWebHookUrl>(d => d.WebHookUrlId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReserveWebHookUrl_WebHookUrl");
            });

            modelBuilder.Entity<ServerWebApp>(entity =>
            {
                entity.Property(e => e.Ip)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ServerName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.WanIp)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.CreateTimeStamp).HasColumnType("datetime");

                entity.Property(e => e.StartTimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.Service)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("FK_Service_Invoice");

                entity.HasOne(d => d.ServiceType)
                    .WithMany(p => p.Service)
                    .HasForeignKey(d => d.ServiceTypeId)
                    .HasConstraintName("FK_ServiceList_ServiceType");
            });

            modelBuilder.Entity<ServiceBotHistory>(entity =>
            {
                entity.HasKey(e => e.ServiceId);

                entity.Property(e => e.ServiceId).ValueGeneratedNever();

                entity.HasOne(d => d.Bot)
                    .WithMany(p => p.ServiceBotHistory)
                    .HasForeignKey(d => d.BotId)
                    .HasConstraintName("FK_ServiceBotHistory_Bot");

                entity.HasOne(d => d.Service)
                    .WithOne(p => p.ServiceBotHistory)
                    .HasForeignKey<ServiceBotHistory>(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceBotHistory_Service");
            });

            modelBuilder.Entity<ServiceType>(entity =>
            {
                entity.Property(e => e.Comment)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WebApp>(entity =>
            {
                entity.Property(e => e.Port)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ServerWebApp)
                    .WithMany(p => p.WebApp)
                    .HasForeignKey(d => d.ServerWebAppId)
                    .HasConstraintName("FK_WebApp_Server");
            });

            modelBuilder.Entity<WebAppHistory>(entity =>
            {
                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.Bot)
                    .WithMany(p => p.WebAppHistory)
                    .HasForeignKey(d => d.BotId)
                    .HasConstraintName("FK_WebAppHistory_Bot");

                entity.HasOne(d => d.WebApp)
                    .WithMany(p => p.WebAppHistory)
                    .HasForeignKey(d => d.WebAppId)
                    .HasConstraintName("FK_WebAppHistory_WebApp");
            });

            modelBuilder.Entity<WebHookUrl>(entity =>
            {
                entity.Property(e => e.Controller)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Dns)
                    .WithMany(p => p.WebHookUrl)
                    .HasForeignKey(d => d.DnsId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WebHookUrl_DNS");

                entity.HasOne(d => d.Port)
                    .WithMany(p => p.WebHookUrl)
                    .HasForeignKey(d => d.PortId)
                    .HasConstraintName("FK_WebHookUrl_WebHookPort");
            });

            modelBuilder.Entity<WebHookUrlHistory>(entity =>
            {
                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.HasOne(d => d.Bot)
                    .WithMany(p => p.WebHookUrlHistory)
                    .HasForeignKey(d => d.BotId)
                    .HasConstraintName("FK_WebHookUrlHistory_Bot");

                entity.HasOne(d => d.WebHookUrl)
                    .WithMany(p => p.WebHookUrlHistory)
                    .HasForeignKey(d => d.WebHookUrlId)
                    .HasConstraintName("FK_WebHookUrlHistory_WebHookUrl");
            });
        }
    }
}
