using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace P01_StudentSystem.Migrations
{
    public partial class DataSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "Description", "EndDate", "Name", "Price", "StartDate" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "MSSQL Server", 200m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Entity Framework Core", 280m, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "Birthday", "Name", "PhoneNumber", "RegisteredOn" },
                values: new object[,]
                {
                    { 1, null, "Michael Henderson", null, new DateTime(2019, 10, 29, 18, 38, 7, 193, DateTimeKind.Local).AddTicks(1667) },
                    { 2, null, "Kate Milner", null, new DateTime(2019, 10, 29, 18, 38, 7, 199, DateTimeKind.Local).AddTicks(3549) }
                });

            migrationBuilder.InsertData(
                table: "HomeworkSubmissions",
                columns: new[] { "HomeworkId", "Content", "ContentType", "CourseId", "StudentId", "SubmissionTime" },
                values: new object[] { 1, null, 2, 1, 2, new DateTime(2019, 10, 29, 18, 38, 7, 202, DateTimeKind.Local).AddTicks(7046) });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "ResourceId", "CourseId", "Name", "ResourceType", "Url" },
                values: new object[] { 2, 2, "Course introduction", 1, null });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "ResourceId", "CourseId", "Name", "ResourceType", "Url" },
                values: new object[] { 1, 1, "Course resources", 2, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HomeworkSubmissions",
                keyColumn: "HomeworkId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 2);
        }
    }
}
