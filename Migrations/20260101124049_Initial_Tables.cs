using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Settings");

            migrationBuilder.EnsureSchema(
                name: "App");

            migrationBuilder.EnsureSchema(
                name: "General");

            migrationBuilder.CreateTable(
                name: "Company",
                schema: "General",
                columns: table => new
                {
                    company_code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    company_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    company_ntn = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    company_is_tax_applicable = table.Column<bool>(type: "bit", nullable: false),
                    company_is_tax_exemption = table.Column<bool>(type: "bit", nullable: false),
                    company_gl_code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    company_functional_currency = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: true),
                    company_default_bank_acc_code = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: true),
                    company_default_bank_code = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.company_code);
                });

            migrationBuilder.CreateTable(
                name: "Designation",
                schema: "General",
                columns: table => new
                {
                    designation_code = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    designation_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designation", x => x.designation_code);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                schema: "General",
                columns: table => new
                {
                    device_id = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    device_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    device_ip_address = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    device_port = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    device_location = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    device_status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    device_last_sync = table.Column<DateTime>(type: "datetime2", nullable: true),
                    device_machine_number = table.Column<int>(type: "int", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.device_id);
                });

            migrationBuilder.CreateTable(
                name: "Holiday",
                schema: "App",
                columns: table => new
                {
                    holiday_day = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    holiday_month = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    holiday_year = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    holiday_fortnight = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    holiday_weekday = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    holiday_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    holiday_title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => new { x.holiday_day, x.holiday_month, x.holiday_year });
                });

            migrationBuilder.CreateTable(
                name: "LeavesType",
                schema: "App",
                columns: table => new
                {
                    leave_type_code = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: false),
                    leave_type_title = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    leave_type_no_of_days = table.Column<int>(type: "int", maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeavesType", x => x.leave_type_code);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "App",
                columns: table => new
                {
                    notification_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    notification_user_code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    notification_title = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    notification_message = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    notification_type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    notification_is_read = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    notification_createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    notification_check_type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    notification_check_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    notification_location = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.notification_id);
                });

            migrationBuilder.CreateTable(
                name: "RequisitionPurposeType",
                schema: "App",
                columns: table => new
                {
                    requisition_type_id = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    requisition_purpose = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitionPurposeType", x => x.requisition_type_id);
                });

            migrationBuilder.CreateTable(
                name: "RoleType",
                schema: "General",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    role_title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    role_is_hidden = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleType", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "StandardHourGroup",
                schema: "App",
                columns: table => new
                {
                    shg_code = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    shg_title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardHourGroup", x => x.shg_code);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                schema: "App",
                columns: table => new
                {
                    task_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    task_title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    task_description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    task_assigned_by = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    task_assigned_by_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    task_assigned_to = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    task_assigned_to_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    task_assign_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    task_due_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    task_is_active = table.Column<bool>(type: "bit", nullable: false),
                    task_is_deleted = table.Column<bool>(type: "bit", nullable: true),
                    task_priority = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    task_status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.task_id);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                schema: "General",
                columns: table => new
                {
                    department_code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    department_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    department_lead_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    department_is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    department_inactive_from_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    department_company_code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.department_code);
                    table.ForeignKey(
                        name: "FK_Department_Company_department_company_code",
                        column: x => x.department_company_code,
                        principalSchema: "General",
                        principalTable: "Company",
                        principalColumn: "company_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StandardHour",
                schema: "App",
                columns: table => new
                {
                    standard_hour_group_code = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    standard_hour_week_day = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    standard_hour_week_day_name = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    standard_hour_hours = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    standard_hour_minutes = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    standard_hour_time_in = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    standard_hour_time_out = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    standard_hour_half_day_min = table.Column<int>(type: "int", maxLength: 4, nullable: true),
                    standard_hour_half_day_on_late_arrival = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardHour", x => new { x.standard_hour_group_code, x.standard_hour_week_day });
                    table.ForeignKey(
                        name: "FK_StandardHour_StandardHourGroup_standard_hour_group_code",
                        column: x => x.standard_hour_group_code,
                        principalSchema: "App",
                        principalTable: "StandardHourGroup",
                        principalColumn: "shg_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskTimeLogs",
                schema: "App",
                columns: table => new
                {
                    time_log_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    time_log_task_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    time_log_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    time_log_start_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    time_log_stop_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    time_log_hours_worked = table.Column<double>(type: "float", nullable: true),
                    time_log_notes = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTimeLogs", x => x.time_log_id);
                    table.ForeignKey(
                        name: "FK_TaskTimeLogs_Task_time_log_task_id",
                        column: x => x.time_log_task_id,
                        principalSchema: "App",
                        principalTable: "Task",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                schema: "General",
                columns: table => new
                {
                    employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    employee_zk_user_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    employee_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    employee_lead_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    employee_company_code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    employee_email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    employee_phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    employee_department_code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    employee_designation_code = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    employee_standard_hour_code = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    employee_cardnumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    employee_is_active = table.Column<bool>(type: "bit", nullable: false),
                    employee_fcm_token = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    employee_join_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    employee_leave_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    employee_created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.employee_code);
                    table.ForeignKey(
                        name: "FK_Employee_Company_employee_company_code",
                        column: x => x.employee_company_code,
                        principalSchema: "General",
                        principalTable: "Company",
                        principalColumn: "company_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Department_employee_department_code",
                        column: x => x.employee_department_code,
                        principalSchema: "General",
                        principalTable: "Department",
                        principalColumn: "department_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_Designation_employee_designation_code",
                        column: x => x.employee_designation_code,
                        principalSchema: "General",
                        principalTable: "Designation",
                        principalColumn: "designation_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employee_StandardHourGroup_employee_standard_hour_code",
                        column: x => x.employee_standard_hour_code,
                        principalSchema: "App",
                        principalTable: "StandardHourGroup",
                        principalColumn: "shg_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppUser",
                schema: "Settings",
                columns: table => new
                {
                    user_code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    user_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    user_employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    user_is_active = table.Column<bool>(type: "bit", nullable: false),
                    user_password = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    user_salt = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.user_code);
                    table.ForeignKey(
                        name: "FK_AppUser_Employee_user_employee_code",
                        column: x => x.user_employee_code,
                        principalSchema: "General",
                        principalTable: "Employee",
                        principalColumn: "employee_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecord",
                schema: "App",
                columns: table => new
                {
                    att_rec_employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    att_rec_year = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    att_rec_month = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    att_rec_fortnight = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    att_rec_day = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    att_rec_standard_hour_code = table.Column<int>(type: "int", maxLength: 4, nullable: true),
                    att_rec_standard_hour_weekday = table.Column<int>(type: "int", maxLength: 4, nullable: true),
                    att_rec_attendance_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    att_rec_check_in_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    att_rec_check_out_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    att_rec_check_in_source = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    att_rec_check_out_source = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    att_rec_check_in_latitude = table.Column<double>(type: "float", nullable: true),
                    att_rec_check_in_longitude = table.Column<double>(type: "float", nullable: true),
                    att_rec_check_out_latitude = table.Column<double>(type: "float", nullable: true),
                    att_rec_check_out_longitude = table.Column<double>(type: "float", nullable: true),
                    att_rec_check_in_photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    att_rec_check_out_photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    att_rec_total_hours = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    att_rec_status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    att_rec_device_id = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    att_rec_device_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    att_rec_location = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecord", x => new { x.att_rec_employee_code, x.att_rec_day, x.att_rec_month, x.att_rec_year, x.att_rec_fortnight });
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_Employee_att_rec_employee_code",
                        column: x => x.att_rec_employee_code,
                        principalSchema: "General",
                        principalTable: "Employee",
                        principalColumn: "employee_code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceRecord_StandardHour_att_rec_standard_hour_code_att_rec_standard_hour_weekday",
                        columns: x => new { x.att_rec_standard_hour_code, x.att_rec_standard_hour_weekday },
                        principalSchema: "App",
                        principalTable: "StandardHour",
                        principalColumns: new[] { "standard_hour_group_code", "standard_hour_week_day" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveBalance",
                schema: "App",
                columns: table => new
                {
                    leave_balance_employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    leave_balance_period = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    leave_balance_leave_type = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    leave_balance_balance = table.Column<decimal>(type: "decimal(18,2)", maxLength: 9, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveBalance", x => new { x.leave_balance_employee_code, x.leave_balance_period, x.leave_balance_leave_type });
                    table.ForeignKey(
                        name: "FK_LeaveBalance_Employee_leave_balance_employee_code",
                        column: x => x.leave_balance_employee_code,
                        principalSchema: "General",
                        principalTable: "Employee",
                        principalColumn: "employee_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequisitionMaster",
                schema: "App",
                columns: table => new
                {
                    leave_requisition_master_id = table.Column<int>(type: "int", maxLength: 4, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    leave_requisition_master_display_id = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    leave_requisition_master_employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    leave_requisition_master_status = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: false),
                    leave_requisition_master_submission_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    leave_requisition_master_approved_date = table.Column<DateTime>(type: "datetime2", unicode: false, maxLength: 20, nullable: true),
                    leave_requisition_master_approver_employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    leave_requisition_master_previous_hajj_year = table.Column<int>(type: "int", maxLength: 4, nullable: true),
                    leave_requisition_master_rejection_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequisitionMaster", x => x.leave_requisition_master_id);
                    table.ForeignKey(
                        name: "FK_LeaveRequisitionMaster_Employee_leave_requisition_master_employee_code",
                        column: x => x.leave_requisition_master_employee_code,
                        principalSchema: "General",
                        principalTable: "Employee",
                        principalColumn: "employee_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "App",
                columns: table => new
                {
                    refresh_token_user_code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.refresh_token_user_code);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_AppUser_refresh_token_user_code",
                        column: x => x.refresh_token_user_code,
                        principalSchema: "Settings",
                        principalTable: "AppUser",
                        principalColumn: "user_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "App",
                columns: table => new
                {
                    user_role_user_Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    user_role_role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.user_role_user_Code, x.user_role_role_id });
                    table.ForeignKey(
                        name: "FK_UserRole_AppUser_user_role_user_Code",
                        column: x => x.user_role_user_Code,
                        principalSchema: "Settings",
                        principalTable: "AppUser",
                        principalColumn: "user_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRole_RoleType_user_role_role_id",
                        column: x => x.user_role_role_id,
                        principalSchema: "General",
                        principalTable: "RoleType",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceLeave",
                schema: "App",
                columns: table => new
                {
                    att_leave_employee_code = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    att_leave_year = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    att_leave_month = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    att_leave_day = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    att_leave_fortnight = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    att_leave_date = table.Column<DateTime>(type: "datetime2", maxLength: 8, nullable: false),
                    att_leave_type_code = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: false),
                    att_leave_hour = table.Column<string>(type: "varchar(5)", unicode: false, maxLength: 5, nullable: true),
                    att_leave_minutes = table.Column<int>(type: "int", maxLength: 4, nullable: true),
                    att_leave_requisition_number = table.Column<int>(type: "int", maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceLeave", x => new { x.att_leave_employee_code, x.att_leave_day, x.att_leave_month, x.att_leave_year, x.att_leave_fortnight });
                    table.ForeignKey(
                        name: "FK_AttendanceLeave_AttendanceRecord_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_fortnight_att_leave_year",
                        columns: x => new { x.att_leave_employee_code, x.att_leave_day, x.att_leave_month, x.att_leave_fortnight, x.att_leave_year },
                        principalSchema: "App",
                        principalTable: "AttendanceRecord",
                        principalColumns: new[] { "att_rec_employee_code", "att_rec_day", "att_rec_month", "att_rec_year", "att_rec_fortnight" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceLeave_Employee_att_leave_employee_code",
                        column: x => x.att_leave_employee_code,
                        principalSchema: "General",
                        principalTable: "Employee",
                        principalColumn: "employee_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceLeave_LeavesType_att_leave_type_code",
                        column: x => x.att_leave_type_code,
                        principalSchema: "App",
                        principalTable: "LeavesType",
                        principalColumn: "leave_type_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequisitionBalance",
                schema: "App",
                columns: table => new
                {
                    leave_requisition_balance_id = table.Column<int>(type: "int", maxLength: 4, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    leave_requisition_balance = table.Column<decimal>(type: "decimal(18,2)", maxLength: 9, nullable: false),
                    leave_requisition_balance_master_id = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    leave_requisition_current_balance = table.Column<decimal>(type: "decimal(18,2)", maxLength: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequisitionBalance", x => x.leave_requisition_balance_id);
                    table.ForeignKey(
                        name: "FK_LeaveRequisitionBalance_LeaveRequisitionMaster_leave_requisition_balance_master_id",
                        column: x => x.leave_requisition_balance_master_id,
                        principalSchema: "App",
                        principalTable: "LeaveRequisitionMaster",
                        principalColumn: "leave_requisition_master_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequisitionDetail",
                schema: "App",
                columns: table => new
                {
                    leave_requisition_detail_id = table.Column<int>(type: "int", maxLength: 4, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    leave_requisition_detail_display_id = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    leave_requisition_detail_master_id = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    leave_requisition_detail_from_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    leave_requisition_detail_to_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    leave_requisition_detail_leave_type_code = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    leave_requisition_detail_no_of_days = table.Column<decimal>(type: "decimal(18,2)", maxLength: 9, nullable: false),
                    leave_requisition_detail_purpose = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    leave_requisition_detail_adjustment_type = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: false),
                    leave_requisition_detail_purpose_code = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    leave_requisition_detail_rejection_reason = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequisitionDetail", x => x.leave_requisition_detail_id);
                    table.ForeignKey(
                        name: "FK_LeaveRequisitionDetail_LeaveRequisitionMaster_leave_requisition_detail_master_id",
                        column: x => x.leave_requisition_detail_master_id,
                        principalSchema: "App",
                        principalTable: "LeaveRequisitionMaster",
                        principalColumn: "leave_requisition_master_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_user_employee_code",
                schema: "Settings",
                table: "AppUser",
                column: "user_employee_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLeave_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_fortnight_att_leave_year",
                schema: "App",
                table: "AttendanceLeave",
                columns: new[] { "att_leave_employee_code", "att_leave_day", "att_leave_month", "att_leave_fortnight", "att_leave_year" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLeave_att_leave_type_code",
                schema: "App",
                table: "AttendanceLeave",
                column: "att_leave_type_code");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecord_att_rec_standard_hour_code_att_rec_standard_hour_weekday",
                schema: "App",
                table: "AttendanceRecord",
                columns: new[] { "att_rec_standard_hour_code", "att_rec_standard_hour_weekday" });

            migrationBuilder.CreateIndex(
                name: "IX_Department_department_company_code",
                schema: "General",
                table: "Department",
                column: "department_company_code");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_employee_company_code",
                schema: "General",
                table: "Employee",
                column: "employee_company_code");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_employee_department_code",
                schema: "General",
                table: "Employee",
                column: "employee_department_code");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_employee_designation_code",
                schema: "General",
                table: "Employee",
                column: "employee_designation_code");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_employee_standard_hour_code",
                schema: "General",
                table: "Employee",
                column: "employee_standard_hour_code");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequisitionBalance_leave_requisition_balance_master_id",
                schema: "App",
                table: "LeaveRequisitionBalance",
                column: "leave_requisition_balance_master_id");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequisitionDetail_leave_requisition_detail_master_id",
                schema: "App",
                table: "LeaveRequisitionDetail",
                column: "leave_requisition_detail_master_id");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequisitionMaster_leave_requisition_master_employee_code",
                schema: "App",
                table: "LeaveRequisitionMaster",
                column: "leave_requisition_master_employee_code");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTimeLogs_time_log_task_id",
                schema: "App",
                table: "TaskTimeLogs",
                column: "time_log_task_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_user_role_role_id",
                schema: "App",
                table: "UserRole",
                column: "user_role_role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceLeave",
                schema: "App");

            migrationBuilder.DropTable(
                name: "Devices",
                schema: "General");

            migrationBuilder.DropTable(
                name: "Holiday",
                schema: "App");

            migrationBuilder.DropTable(
                name: "LeaveBalance",
                schema: "App");

            migrationBuilder.DropTable(
                name: "LeaveRequisitionBalance",
                schema: "App");

            migrationBuilder.DropTable(
                name: "LeaveRequisitionDetail",
                schema: "App");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "App");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "App");

            migrationBuilder.DropTable(
                name: "RequisitionPurposeType",
                schema: "App");

            migrationBuilder.DropTable(
                name: "TaskTimeLogs",
                schema: "App");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "App");

            migrationBuilder.DropTable(
                name: "AttendanceRecord",
                schema: "App");

            migrationBuilder.DropTable(
                name: "LeavesType",
                schema: "App");

            migrationBuilder.DropTable(
                name: "LeaveRequisitionMaster",
                schema: "App");

            migrationBuilder.DropTable(
                name: "Task",
                schema: "App");

            migrationBuilder.DropTable(
                name: "AppUser",
                schema: "Settings");

            migrationBuilder.DropTable(
                name: "RoleType",
                schema: "General");

            migrationBuilder.DropTable(
                name: "StandardHour",
                schema: "App");

            migrationBuilder.DropTable(
                name: "Employee",
                schema: "General");

            migrationBuilder.DropTable(
                name: "Department",
                schema: "General");

            migrationBuilder.DropTable(
                name: "Designation",
                schema: "General");

            migrationBuilder.DropTable(
                name: "StandardHourGroup",
                schema: "App");

            migrationBuilder.DropTable(
                name: "Company",
                schema: "General");
        }
    }
}
