using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SWP.Models;

public partial class HiemMuonContext : DbContext
{
    public HiemMuonContext()
    {
    }

    public HiemMuonContext(DbContextOptions<HiemMuonContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<BioSample> BioSamples { get; set; }

    public virtual DbSet<BioType> BioTypes { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorSchedule> DoctorSchedules { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<MethodPayment> MethodPayments { get; set; }

    public virtual DbSet<PaymentBooking> PaymentBookings { get; set; }

    public virtual DbSet<PaymentTreatment> PaymentTreatments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<SlotSchedule> SlotSchedules { get; set; }

    public virtual DbSet<StatusPayment> StatusPayments { get; set; }

    public virtual DbSet<StepDetail> StepDetails { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TreatmentPlan> TreatmentPlans { get; set; }

    public virtual DbSet<TreatmentStep> TreatmentSteps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=MSI\\DBAO;Initial Catalog=HIEM_MUON;User ID=sa;Password=12345;Encrypt=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccId).HasName("PK__Account__49ACB9A450969572");

            entity.ToTable("Account");

            entity.Property(e => e.AccId)
                .ValueGeneratedNever()
                .HasColumnName("Acc_ID");
            entity.Property(e => e.AccName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Acc_Name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("Role_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Account__Passwor__38996AB5");
        });

        modelBuilder.Entity<BioSample>(entity =>
        {
            entity.HasKey(e => e.BsId).HasName("PK__BioSampl__B5A8B265CCDC86A4");

            entity.ToTable("BioSample");

            entity.Property(e => e.BsId)
                .ValueGeneratedNever()
                .HasColumnName("BS_ID");
            entity.Property(e => e.BsName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("BS_Name");
            entity.Property(e => e.BtId).HasColumnName("BT_ID");
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.Quality)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StorageLocation)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TpId).HasColumnName("TP_ID");

            entity.HasOne(d => d.Bt).WithMany(p => p.BioSamples)
                .HasForeignKey(d => d.BtId)
                .HasConstraintName("FK__BioSample__BT_ID__66603565");

            entity.HasOne(d => d.Tp).WithMany(p => p.BioSamples)
                .HasForeignKey(d => d.TpId)
                .HasConstraintName("FK__BioSample__Note__656C112C");
        });

        modelBuilder.Entity<BioType>(entity =>
        {
            entity.HasKey(e => e.BtId).HasName("PK__BioType__9C28A3C28A9B1C76");

            entity.ToTable("BioType");

            entity.Property(e => e.BtId)
                .ValueGeneratedNever()
                .HasColumnName("BT_ID");
            entity.Property(e => e.BtName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("BT_Name");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blog__C164D018D6926C6A");

            entity.ToTable("Blog");

            entity.Property(e => e.BlogId)
                .ValueGeneratedNever()
                .HasColumnName("Blog_ID");
            entity.Property(e => e.AuthorId).HasColumnName("Author_ID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.Img).HasColumnType("text");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Blog__isActive__693CA210");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__35ABFDE0FE2FFF06");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId)
                .ValueGeneratedNever()
                .HasColumnName("Booking_ID");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("Create_At");
            entity.Property(e => e.CusId).HasColumnName("Cus_ID");
            entity.Property(e => e.DocId).HasColumnName("Doc_ID");
            entity.Property(e => e.DsId).HasColumnName("DS_ID");
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Cus).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CusId)
                .HasConstraintName("FK__Booking__Note__49C3F6B7");

            entity.HasOne(d => d.Doc).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.DocId)
                .HasConstraintName("FK__Booking__Doc_ID__4AB81AF0");

            entity.HasOne(d => d.Ds).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.DsId)
                .HasConstraintName("FK__Booking__DS_ID__4BAC3F29");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CusId).HasName("PK__Customer__0AD1655728C03EAF");

            entity.ToTable("Customer");

            entity.Property(e => e.CusId)
                .ValueGeneratedNever()
                .HasColumnName("Cus_ID");
            entity.Property(e => e.AccId).HasColumnName("Acc_ID");
            entity.Property(e => e.HusName)
                .HasMaxLength(100)
                .HasColumnName("Hus_Name");
            entity.Property(e => e.HusYob).HasColumnName("Hus_YOB");
            entity.Property(e => e.Mail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.WifeName)
                .HasMaxLength(100)
                .HasColumnName("Wife_Name");
            entity.Property(e => e.WifeYob).HasColumnName("Wife_YOB");

            entity.HasOne(d => d.Acc).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AccId)
                .HasConstraintName("FK__Customer__Mail__3B75D760");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DocId).HasName("PK__Doctor__46473821F5ED1EA6");

            entity.ToTable("Doctor");

            entity.Property(e => e.DocId)
                .ValueGeneratedNever()
                .HasColumnName("Doc_ID");
            entity.Property(e => e.AccId).HasColumnName("Acc_ID");
            entity.Property(e => e.Certification).HasColumnType("text");
            entity.Property(e => e.DocName)
                .HasMaxLength(100)
                .HasColumnName("Doc_Name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Mail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Specialized)
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Yob).HasColumnName("YOB");

            entity.HasOne(d => d.Acc).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.AccId)
                .HasConstraintName("FK__Doctor__Certific__3E52440B");
        });

        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(e => e.DsId).HasName("PK__DoctorSc__EFC56BC66D080D55");

            entity.ToTable("DoctorSchedule");

            entity.Property(e => e.DsId)
                .ValueGeneratedNever()
                .HasColumnName("DS_ID");
            entity.Property(e => e.DocId).HasColumnName("Doc_ID");
            entity.Property(e => e.IsAvailable).HasColumnName("isAvailable");
            entity.Property(e => e.RoomId).HasColumnName("Room_ID");
            entity.Property(e => e.SlotId).HasColumnName("Slot_ID");

            entity.HasOne(d => d.Doc).WithMany(p => p.DoctorSchedules)
                .HasForeignKey(d => d.DocId)
                .HasConstraintName("FK__DoctorSch__Doc_I__44FF419A");

            entity.HasOne(d => d.Room).WithMany(p => p.DoctorSchedules)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__DoctorSch__Room___46E78A0C");

            entity.HasOne(d => d.Slot).WithMany(p => p.DoctorSchedules)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK__DoctorSch__Slot___45F365D3");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FbId).HasName("PK__Feedback__28DF61717FCBA852");

            entity.ToTable("Feedback");

            entity.Property(e => e.FbId)
                .ValueGeneratedNever()
                .HasColumnName("FB_ID");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.DocId).HasColumnName("Doc_ID");
            entity.Property(e => e.TpId).HasColumnName("TP_ID");

            entity.HasOne(d => d.Doc).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.DocId)
                .HasConstraintName("FK__Feedback__Doc_ID__5CD6CB2B");

            entity.HasOne(d => d.Tp).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.TpId)
                .HasConstraintName("FK__Feedback__TP_ID__5BE2A6F2");
        });

        modelBuilder.Entity<MethodPayment>(entity =>
        {
            entity.HasKey(e => e.MethodId).HasName("PK__Method_P__FB48B3C473FE4AD5");

            entity.ToTable("Method_Payment");

            entity.Property(e => e.MethodId).HasColumnName("Method_ID");
            entity.Property(e => e.MethodName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Method_Name");
        });

        modelBuilder.Entity<PaymentBooking>(entity =>
        {
            entity.HasKey(e => e.PbId).HasName("PK__Payment___624B28C428CCD110");

            entity.ToTable("Payment_Booking");

            entity.Property(e => e.PbId).HasColumnName("PB_ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BookingId).HasColumnName("Booking_ID");
            entity.Property(e => e.MethodId).HasColumnName("Method_ID");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("Status_ID");

            entity.HasOne(d => d.Booking).WithMany(p => p.PaymentBookings)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment_B__Booki__70DDC3D8");

            entity.HasOne(d => d.Method).WithMany(p => p.PaymentBookings)
                .HasForeignKey(d => d.MethodId)
                .HasConstraintName("FK__Payment_B__Metho__71D1E811");

            entity.HasOne(d => d.Status).WithMany(p => p.PaymentBookings)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Payment_B__Statu__72C60C4A");
        });

        modelBuilder.Entity<PaymentTreatment>(entity =>
        {
            entity.HasKey(e => e.PtId).HasName("PK__Payment___6836DDD3887B9515");

            entity.ToTable("Payment_Treatment");

            entity.Property(e => e.PtId).HasColumnName("PT_ID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MethodId).HasColumnName("Method_ID");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("Status_ID");
            entity.Property(e => e.TpId).HasColumnName("TP_ID");

            entity.HasOne(d => d.Method).WithMany(p => p.PaymentTreatments)
                .HasForeignKey(d => d.MethodId)
                .HasConstraintName("FK__Payment_T__Metho__778AC167");

            entity.HasOne(d => d.Status).WithMany(p => p.PaymentTreatments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Payment_T__Statu__787EE5A0");

            entity.HasOne(d => d.Tp).WithMany(p => p.PaymentTreatments)
                .HasForeignKey(d => d.TpId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment_T__TP_ID__76969D2E");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__D80AB49B05C6C6F8");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("Role_ID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Role_Name");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Room__19EE6A73A545CF97");

            entity.ToTable("Room");

            entity.Property(e => e.RoomId).HasColumnName("Room_ID");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Room_Number");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.SerId).HasName("PK__Services__266F0071B3FF801B");

            entity.Property(e => e.SerId)
                .ValueGeneratedNever()
                .HasColumnName("Ser_ID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SerName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Ser_Name");
        });

        modelBuilder.Entity<SlotSchedule>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__Slot_Sch__1AE2AAAE6325EAD5");

            entity.ToTable("Slot_Schedule");

            entity.Property(e => e.SlotId).HasColumnName("Slot_ID");
            entity.Property(e => e.SlotEnd).HasColumnName("Slot_End");
            entity.Property(e => e.SlotStart).HasColumnName("Slot_Start");
        });

        modelBuilder.Entity<StatusPayment>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Status_P__519009AC974DB028");

            entity.ToTable("Status_Payment");

            entity.Property(e => e.StatusId).HasColumnName("Status_ID");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Status_Name");
        });

        modelBuilder.Entity<StepDetail>(entity =>
        {
            entity.HasKey(e => e.SdId).HasName("PK__StepDeta__DD5A6BA3F1CB6003");

            entity.ToTable("StepDetail");

            entity.Property(e => e.SdId)
                .ValueGeneratedNever()
                .HasColumnName("SD_ID");
            entity.Property(e => e.Dosage)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DrugName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Drug_Name");
            entity.Property(e => e.Note).HasColumnType("text");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StepName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Step_Name");
            entity.Property(e => e.TpId).HasColumnName("TP_ID");
            entity.Property(e => e.TsId).HasColumnName("TS_ID");

            entity.HasOne(d => d.Tp).WithMany(p => p.StepDetails)
                .HasForeignKey(d => d.TpId)
                .HasConstraintName("FK__StepDetai__TP_ID__5FB337D6");

            entity.HasOne(d => d.Ts).WithMany(p => p.StepDetails)
                .HasForeignKey(d => d.TsId)
                .HasConstraintName("FK__StepDetai__TS_ID__60A75C0F");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PK__Test__B502D00279F1B4BC");

            entity.ToTable("Test");

            entity.Property(e => e.TestId)
                .ValueGeneratedNever()
                .HasColumnName("Test_ID");
            entity.Property(e => e.CusId).HasColumnName("Cus_ID");
            entity.Property(e => e.DocId).HasColumnName("Doc_ID");
            entity.Property(e => e.HusTest)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Hus_Test");
            entity.Property(e => e.HusTestDate).HasColumnName("Hus_Test_Date");
            entity.Property(e => e.HusTestResult)
                .HasColumnType("text")
                .HasColumnName("Hus_Test_Result");
            entity.Property(e => e.WifeTest)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Wife_Test");
            entity.Property(e => e.WifeTestDate).HasColumnName("Wife_Test_Date");
            entity.Property(e => e.WifeTestResult)
                .HasColumnType("text")
                .HasColumnName("Wife_Test_Result");

            entity.HasOne(d => d.Cus).WithMany(p => p.Tests)
                .HasForeignKey(d => d.CusId)
                .HasConstraintName("FK__Test__Cus_ID__4F7CD00D");

            entity.HasOne(d => d.Doc).WithMany(p => p.Tests)
                .HasForeignKey(d => d.DocId)
                .HasConstraintName("FK__Test__Doc_ID__4E88ABD4");
        });

        modelBuilder.Entity<TreatmentPlan>(entity =>
        {
            entity.HasKey(e => e.TpId).HasName("PK__Treatmen__8106F2C49E184300");

            entity.Property(e => e.TpId)
                .ValueGeneratedNever()
                .HasColumnName("TP_ID");
            entity.Property(e => e.CurrentStepId).HasColumnName("Current_Step_ID");
            entity.Property(e => e.CusId).HasColumnName("Cus_ID");
            entity.Property(e => e.DocId).HasColumnName("Doc_ID");
            entity.Property(e => e.SerId).HasColumnName("Ser_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CurrentStep).WithMany(p => p.TreatmentPlans)
                .HasForeignKey(d => d.CurrentStepId)
                .HasConstraintName("FK__Treatment__Curre__59063A47");

            entity.HasOne(d => d.Cus).WithMany(p => p.TreatmentPlans)
                .HasForeignKey(d => d.CusId)
                .HasConstraintName("FK__Treatment__Cus_I__571DF1D5");

            entity.HasOne(d => d.Doc).WithMany(p => p.TreatmentPlans)
                .HasForeignKey(d => d.DocId)
                .HasConstraintName("FK__Treatment__Doc_I__5812160E");

            entity.HasOne(d => d.Ser).WithMany(p => p.TreatmentPlans)
                .HasForeignKey(d => d.SerId)
                .HasConstraintName("FK__Treatment__Ser_I__5629CD9C");
        });

        modelBuilder.Entity<TreatmentStep>(entity =>
        {
            entity.HasKey(e => e.TsId).HasName("PK__Treatmen__D128865A719A84E8");

            entity.ToTable("TreatmentStep");

            entity.Property(e => e.TsId)
                .ValueGeneratedNever()
                .HasColumnName("TS_ID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.StepName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Step_Name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
