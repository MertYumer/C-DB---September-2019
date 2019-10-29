namespace P01_StudentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class StudentSystemContext : DbContext
    {
        //You will need a constructor, accepting DbContextOptions to test your solution in Judge
        //public StudentSystemContext(DbContextOptions options)
        //: base(options)
        //{

        //}

        public DbSet<Course> Courses { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=StudentSystem;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureCourseEntity(modelBuilder);

            ConfigureHomeworkEntity(modelBuilder);

            ConfigureResourceEntity(modelBuilder);

            ConfigureStudentEntity(modelBuilder);

            ConfigureStudentCourseEntity(modelBuilder);

            modelBuilder.Seed();
        }

        private void ConfigureCourseEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(course =>
            {
                course.HasKey(c => c.CourseId);

                course
                .HasMany(c => c.StudentsEnrolled)
                .WithOne(se => se.Course);

                course
                .HasMany(c => c.Resources)
                .WithOne(r => r.Course);

                course
                .HasMany(c => c.HomeworkSubmissions)
                .WithOne(hs => hs.Course);

                course
                .Property(c => c.Name)
                .HasMaxLength(80)
                .IsRequired()
                .IsUnicode();

                course
                .Property(c => c.Description)
                .IsRequired(false)
                .IsUnicode();
            });
        }

        private void ConfigureHomeworkEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Homework>(homework =>
            {
                homework.HasKey(h => h.HomeworkId);

                homework
                .Property(h => h.Content)
                .IsUnicode(false);
            });
        }

        private void ConfigureResourceEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>(resource =>
            {
                resource.HasKey(r => r.ResourceId);

                resource
                .Property(r => r.Name)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode();

                resource
                .Property(r => r.Url)
                .IsUnicode(false);
            });
        }

        private void ConfigureStudentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(student =>
            {
                student.HasKey(s => s.StudentId);

                student
                .HasMany(s => s.CourseEnrollments)
                .WithOne(ce => ce.Student);

                student
                .HasMany(s => s.HomeworkSubmissions)
                .WithOne(hs => hs.Student);

                student
                .Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode();

                student
                .Property(c => c.PhoneNumber)
                .HasColumnType("CHAR(10)")
                .IsUnicode(false)
                .IsRequired(false);
            });
        }

        private void ConfigureStudentCourseEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>(studentsCourses =>
            {
                studentsCourses.HasKey(sc => new { sc.StudentId, sc.CourseId });

                studentsCourses
                .HasOne(sc => sc.Student)
                .WithMany(s => s.CourseEnrollments)
                .HasForeignKey(sc => sc.StudentId);

                studentsCourses
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentsEnrolled)
                .HasForeignKey(sc => sc.CourseId);
            });
        }
    }
}
