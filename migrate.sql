IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    IF SCHEMA_ID(N'Settings') IS NULL EXEC(N'CREATE SCHEMA [Settings];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    IF SCHEMA_ID(N'App') IS NULL EXEC(N'CREATE SCHEMA [App];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    IF SCHEMA_ID(N'General') IS NULL EXEC(N'CREATE SCHEMA [General];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [General].[Company] (
        [company_code] nvarchar(4) NOT NULL,
        [company_name] varchar(50) NOT NULL,
        [company_ntn] varchar(20) NULL,
        [company_is_tax_applicable] bit NOT NULL,
        [company_is_tax_exemption] bit NOT NULL,
        [company_gl_code] varchar(10) NULL,
        [company_functional_currency] varchar(4) NULL,
        [company_default_bank_acc_code] varchar(4) NULL,
        [company_default_bank_code] varchar(4) NULL,
        CONSTRAINT [PK_Company] PRIMARY KEY ([company_code])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [General].[Designation] (
        [designation_code] int NOT NULL,
        [designation_name] varchar(50) NOT NULL,
        CONSTRAINT [PK_Designation] PRIMARY KEY ([designation_code])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [General].[Devices] (
        [device_id] nvarchar(20) NOT NULL,
        [device_name] varchar(50) NOT NULL,
        [device_ip_address] varchar(20) NOT NULL,
        [device_port] int NOT NULL,
        [device_location] varchar(100) NOT NULL,
        [device_status] varchar(10) NOT NULL,
        [device_last_sync] datetime2 NULL,
        [device_machine_number] int NOT NULL,
        CONSTRAINT [PK_Devices] PRIMARY KEY ([device_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[Holiday] (
        [holiday_day] int NOT NULL,
        [holiday_month] int NOT NULL,
        [holiday_year] int NOT NULL,
        [holiday_fortnight] int NOT NULL,
        [holiday_weekday] int NOT NULL,
        [holiday_date] datetime2 NOT NULL,
        [holiday_title] varchar(50) NOT NULL,
        CONSTRAINT [PK_Holiday] PRIMARY KEY ([holiday_day], [holiday_month], [holiday_year])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[LeavesType] (
        [leave_type_code] varchar(4) NOT NULL,
        [leave_type_title] varchar(20) NOT NULL,
        [leave_type_no_of_days] int NULL,
        CONSTRAINT [PK_LeavesType] PRIMARY KEY ([leave_type_code])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[Notification] (
        [notification_id] nvarchar(450) NOT NULL,
        [notification_user_code] nvarchar(5) NOT NULL,
        [notification_title] varchar(150) NOT NULL,
        [notification_message] varchar(250) NOT NULL,
        [notification_type] varchar(20) NOT NULL,
        [notification_is_read] bit NOT NULL DEFAULT CAST(0 AS bit),
        [notification_createdAt] datetime2 NOT NULL,
        [notification_check_type] varchar(20) NULL,
        [notification_check_time] datetime2 NOT NULL,
        [notification_location] varchar(200) NULL,
        CONSTRAINT [PK_Notification] PRIMARY KEY ([notification_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[RequisitionPurposeType] (
        [requisition_type_id] int NOT NULL,
        [requisition_purpose] varchar(500) NOT NULL,
        CONSTRAINT [PK_RequisitionPurposeType] PRIMARY KEY ([requisition_type_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [General].[RoleType] (
        [role_id] uniqueidentifier NOT NULL,
        [role_title] varchar(50) NOT NULL,
        [role_is_hidden] bit NOT NULL,
        CONSTRAINT [PK_RoleType] PRIMARY KEY ([role_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[StandardHourGroup] (
        [shg_code] int NOT NULL,
        [shg_title] varchar(50) NOT NULL,
        CONSTRAINT [PK_StandardHourGroup] PRIMARY KEY ([shg_code])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[Task] (
        [task_id] uniqueidentifier NOT NULL,
        [task_title] varchar(50) NOT NULL,
        [task_description] varchar(max) NOT NULL,
        [task_assigned_by] varchar(50) NOT NULL,
        [task_assigned_by_name] varchar(50) NULL,
        [task_assigned_to] varchar(50) NOT NULL,
        [task_assigned_to_name] varchar(50) NULL,
        [task_assign_date] datetime2 NOT NULL,
        [task_due_date] datetime2 NULL,
        [task_is_active] bit NOT NULL,
        [task_is_deleted] bit NULL,
        [task_priority] varchar(10) NOT NULL,
        [task_status] varchar(10) NOT NULL,
        CONSTRAINT [PK_Task] PRIMARY KEY ([task_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [General].[Department] (
        [department_code] nvarchar(8) NOT NULL,
        [department_name] varchar(50) NOT NULL,
        [department_lead_code] nvarchar(20) NULL,
        [department_is_active] bit NOT NULL DEFAULT CAST(1 AS bit),
        [department_inactive_from_date] datetime2 NULL,
        [department_company_code] nvarchar(4) NOT NULL,
        CONSTRAINT [PK_Department] PRIMARY KEY ([department_code]),
        CONSTRAINT [FK_Department_Company_department_company_code] FOREIGN KEY ([department_company_code]) REFERENCES [General].[Company] ([company_code]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[StandardHour] (
        [standard_hour_group_code] int NOT NULL,
        [standard_hour_week_day] int NOT NULL,
        [standard_hour_week_day_name] varchar(3) NOT NULL,
        [standard_hour_hours] varchar(5) NOT NULL,
        [standard_hour_minutes] int NOT NULL,
        [standard_hour_time_in] varchar(5) NULL,
        [standard_hour_time_out] varchar(5) NULL,
        [standard_hour_half_day_min] int NULL,
        [standard_hour_half_day_on_late_arrival] varchar(5) NULL,
        CONSTRAINT [PK_StandardHour] PRIMARY KEY ([standard_hour_group_code], [standard_hour_week_day]),
        CONSTRAINT [FK_StandardHour_StandardHourGroup_standard_hour_group_code] FOREIGN KEY ([standard_hour_group_code]) REFERENCES [App].[StandardHourGroup] ([shg_code]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[TaskTimeLogs] (
        [time_log_id] uniqueidentifier NOT NULL,
        [time_log_task_id] uniqueidentifier NOT NULL,
        [time_log_date] datetime2 NOT NULL,
        [time_log_start_time] datetime2 NULL,
        [time_log_stop_time] datetime2 NULL,
        [time_log_hours_worked] float NULL,
        [time_log_notes] nvarchar(100) NULL,
        CONSTRAINT [PK_TaskTimeLogs] PRIMARY KEY ([time_log_id]),
        CONSTRAINT [FK_TaskTimeLogs_Task_time_log_task_id] FOREIGN KEY ([time_log_task_id]) REFERENCES [App].[Task] ([task_id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [General].[Employee] (
        [employee_code] varchar(20) NOT NULL,
        [employee_zk_user_id] nvarchar(10) NOT NULL,
        [employee_name] varchar(50) NOT NULL,
        [employee_lead_code] nvarchar(20) NULL,
        [employee_company_code] nvarchar(4) NOT NULL,
        [employee_email] varchar(50) NOT NULL,
        [employee_phone] varchar(20) NOT NULL,
        [employee_department_code] nvarchar(8) NOT NULL,
        [employee_designation_code] int NOT NULL,
        [employee_standard_hour_code] int NOT NULL,
        [employee_cardnumber] varchar(20) NULL,
        [employee_is_active] bit NOT NULL,
        [employee_fcm_token] varchar(max) NULL,
        [employee_join_date] datetime2 NOT NULL,
        [employee_leave_date] datetime2 NULL,
        [employee_created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_Employee] PRIMARY KEY ([employee_code]),
        CONSTRAINT [FK_Employee_Company_employee_company_code] FOREIGN KEY ([employee_company_code]) REFERENCES [General].[Company] ([company_code]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Employee_Department_employee_department_code] FOREIGN KEY ([employee_department_code]) REFERENCES [General].[Department] ([department_code]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Employee_Designation_employee_designation_code] FOREIGN KEY ([employee_designation_code]) REFERENCES [General].[Designation] ([designation_code]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Employee_StandardHourGroup_employee_standard_hour_code] FOREIGN KEY ([employee_standard_hour_code]) REFERENCES [App].[StandardHourGroup] ([shg_code]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [Settings].[AppUser] (
        [user_code] varchar(50) NOT NULL,
        [user_name] varchar(50) NOT NULL,
        [user_employee_code] varchar(20) NOT NULL,
        [user_is_active] bit NOT NULL,
        [user_password] varchar(max) NOT NULL,
        [user_salt] varchar(max) NOT NULL,
        CONSTRAINT [PK_AppUser] PRIMARY KEY ([user_code]),
        CONSTRAINT [FK_AppUser_Employee_user_employee_code] FOREIGN KEY ([user_employee_code]) REFERENCES [General].[Employee] ([employee_code]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[AttendanceRecord] (
        [att_rec_employee_code] varchar(20) NOT NULL,
        [att_rec_year] int NOT NULL,
        [att_rec_month] int NOT NULL,
        [att_rec_fortnight] int NOT NULL,
        [att_rec_day] int NOT NULL,
        [att_rec_standard_hour_code] int NULL,
        [att_rec_standard_hour_weekday] int NULL,
        [att_rec_attendance_date] datetime2 NOT NULL,
        [att_rec_check_in_time] datetime2 NULL,
        [att_rec_check_out_time] datetime2 NULL,
        [att_rec_check_in_source] nvarchar(10) NOT NULL,
        [att_rec_check_out_source] nvarchar(10) NOT NULL,
        [att_rec_check_in_latitude] float NULL,
        [att_rec_check_in_longitude] float NULL,
        [att_rec_check_out_latitude] float NULL,
        [att_rec_check_out_longitude] float NULL,
        [att_rec_check_in_photo] nvarchar(max) NULL,
        [att_rec_check_out_photo] nvarchar(max) NULL,
        [att_rec_total_hours] nvarchar(10) NULL,
        [att_rec_status] nvarchar(10) NULL,
        [att_rec_device_id] nvarchar(10) NULL,
        [att_rec_device_name] nvarchar(10) NULL,
        [att_rec_location] nvarchar(10) NULL,
        CONSTRAINT [PK_AttendanceRecord] PRIMARY KEY ([att_rec_employee_code], [att_rec_day], [att_rec_month], [att_rec_year], [att_rec_fortnight]),
        CONSTRAINT [FK_AttendanceRecord_Employee_att_rec_employee_code] FOREIGN KEY ([att_rec_employee_code]) REFERENCES [General].[Employee] ([employee_code]) ON DELETE CASCADE,
        CONSTRAINT [FK_AttendanceRecord_StandardHour_att_rec_standard_hour_code_att_rec_standard_hour_weekday] FOREIGN KEY ([att_rec_standard_hour_code], [att_rec_standard_hour_weekday]) REFERENCES [App].[StandardHour] ([standard_hour_group_code], [standard_hour_week_day]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[LeaveBalance] (
        [leave_balance_employee_code] varchar(20) NOT NULL,
        [leave_balance_period] int NOT NULL,
        [leave_balance_leave_type] varchar(5) NOT NULL,
        [leave_balance_balance] decimal(18,2) NULL,
        CONSTRAINT [PK_LeaveBalance] PRIMARY KEY ([leave_balance_employee_code], [leave_balance_period], [leave_balance_leave_type]),
        CONSTRAINT [FK_LeaveBalance_Employee_leave_balance_employee_code] FOREIGN KEY ([leave_balance_employee_code]) REFERENCES [General].[Employee] ([employee_code]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[LeaveRequisitionMaster] (
        [leave_requisition_master_id] int NOT NULL IDENTITY,
        [leave_requisition_master_display_id] varchar(10) NOT NULL,
        [leave_requisition_master_employee_code] varchar(20) NOT NULL,
        [leave_requisition_master_status] varchar(5) NOT NULL,
        [leave_requisition_master_submission_date] datetime2 NULL,
        [leave_requisition_master_approved_date] datetime2 NULL,
        [leave_requisition_master_approver_employee_code] varchar(20) NOT NULL,
        [leave_requisition_master_previous_hajj_year] int NULL,
        [leave_requisition_master_rejection_date] datetime2 NULL,
        CONSTRAINT [PK_LeaveRequisitionMaster] PRIMARY KEY ([leave_requisition_master_id]),
        CONSTRAINT [FK_LeaveRequisitionMaster_Employee_leave_requisition_master_employee_code] FOREIGN KEY ([leave_requisition_master_employee_code]) REFERENCES [General].[Employee] ([employee_code]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[refresh_tokens] (
        [refresh_token_user_code] varchar(50) NOT NULL,
        [refresh_token] nvarchar(500) NOT NULL,
        [expires_at] datetime2 NOT NULL,
        [created_at] datetime2 NOT NULL,
        CONSTRAINT [PK_refresh_tokens] PRIMARY KEY ([refresh_token_user_code]),
        CONSTRAINT [FK_refresh_tokens_AppUser_refresh_token_user_code] FOREIGN KEY ([refresh_token_user_code]) REFERENCES [Settings].[AppUser] ([user_code]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[UserRole] (
        [user_role_user_Code] varchar(50) NOT NULL,
        [user_role_role_id] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserRole] PRIMARY KEY ([user_role_user_Code], [user_role_role_id]),
        CONSTRAINT [FK_UserRole_AppUser_user_role_user_Code] FOREIGN KEY ([user_role_user_Code]) REFERENCES [Settings].[AppUser] ([user_code]) ON DELETE NO ACTION,
        CONSTRAINT [FK_UserRole_RoleType_user_role_role_id] FOREIGN KEY ([user_role_role_id]) REFERENCES [General].[RoleType] ([role_id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[AttendanceLeave] (
        [att_leave_employee_code] varchar(20) NOT NULL,
        [att_leave_year] int NOT NULL,
        [att_leave_month] int NOT NULL,
        [att_leave_day] int NOT NULL,
        [att_leave_fortnight] int NOT NULL,
        [att_leave_date] datetime2 NOT NULL,
        [att_leave_type_code] varchar(4) NOT NULL,
        [att_leave_hour] varchar(5) NULL,
        [att_leave_minutes] int NULL,
        [att_leave_requisition_number] int NULL,
        CONSTRAINT [PK_AttendanceLeave] PRIMARY KEY ([att_leave_employee_code], [att_leave_day], [att_leave_month], [att_leave_year], [att_leave_fortnight]),
        CONSTRAINT [FK_AttendanceLeave_AttendanceRecord_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_fortnight_att_leave_year] FOREIGN KEY ([att_leave_employee_code], [att_leave_day], [att_leave_month], [att_leave_fortnight], [att_leave_year]) REFERENCES [App].[AttendanceRecord] ([att_rec_employee_code], [att_rec_day], [att_rec_month], [att_rec_year], [att_rec_fortnight]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceLeave_Employee_att_leave_employee_code] FOREIGN KEY ([att_leave_employee_code]) REFERENCES [General].[Employee] ([employee_code]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AttendanceLeave_LeavesType_att_leave_type_code] FOREIGN KEY ([att_leave_type_code]) REFERENCES [App].[LeavesType] ([leave_type_code]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[LeaveRequisitionBalance] (
        [leave_requisition_balance_id] int NOT NULL IDENTITY,
        [leave_requisition_balance] decimal(18,2) NOT NULL,
        [leave_requisition_balance_master_id] int NOT NULL,
        [leave_requisition_current_balance] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_LeaveRequisitionBalance] PRIMARY KEY ([leave_requisition_balance_id]),
        CONSTRAINT [FK_LeaveRequisitionBalance_LeaveRequisitionMaster_leave_requisition_balance_master_id] FOREIGN KEY ([leave_requisition_balance_master_id]) REFERENCES [App].[LeaveRequisitionMaster] ([leave_requisition_master_id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE TABLE [App].[LeaveRequisitionDetail] (
        [leave_requisition_detail_id] int NOT NULL IDENTITY,
        [leave_requisition_detail_display_id] varchar(10) NOT NULL,
        [leave_requisition_detail_master_id] int NOT NULL,
        [leave_requisition_detail_from_date] datetime2 NOT NULL,
        [leave_requisition_detail_to_date] datetime2 NOT NULL,
        [leave_requisition_detail_leave_type_code] varchar(3) NOT NULL,
        [leave_requisition_detail_no_of_days] decimal(18,2) NOT NULL,
        [leave_requisition_detail_purpose] varchar(200) NOT NULL,
        [leave_requisition_detail_adjustment_type] varchar(2) NOT NULL,
        [leave_requisition_detail_purpose_code] int NOT NULL,
        [leave_requisition_detail_rejection_reason] varchar(200) NOT NULL,
        CONSTRAINT [PK_LeaveRequisitionDetail] PRIMARY KEY ([leave_requisition_detail_id]),
        CONSTRAINT [FK_LeaveRequisitionDetail_LeaveRequisitionMaster_leave_requisition_detail_master_id] FOREIGN KEY ([leave_requisition_detail_master_id]) REFERENCES [App].[LeaveRequisitionMaster] ([leave_requisition_master_id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AppUser_user_employee_code] ON [Settings].[AppUser] ([user_employee_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_AttendanceLeave_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_fortnight_att_leave_year] ON [App].[AttendanceLeave] ([att_leave_employee_code], [att_leave_day], [att_leave_month], [att_leave_fortnight], [att_leave_year]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_AttendanceLeave_att_leave_type_code] ON [App].[AttendanceLeave] ([att_leave_type_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_AttendanceRecord_att_rec_standard_hour_code_att_rec_standard_hour_weekday] ON [App].[AttendanceRecord] ([att_rec_standard_hour_code], [att_rec_standard_hour_weekday]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_Department_department_company_code] ON [General].[Department] ([department_company_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_Employee_employee_company_code] ON [General].[Employee] ([employee_company_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_Employee_employee_department_code] ON [General].[Employee] ([employee_department_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_Employee_employee_designation_code] ON [General].[Employee] ([employee_designation_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_Employee_employee_standard_hour_code] ON [General].[Employee] ([employee_standard_hour_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_LeaveRequisitionBalance_leave_requisition_balance_master_id] ON [App].[LeaveRequisitionBalance] ([leave_requisition_balance_master_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_LeaveRequisitionDetail_leave_requisition_detail_master_id] ON [App].[LeaveRequisitionDetail] ([leave_requisition_detail_master_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_LeaveRequisitionMaster_leave_requisition_master_employee_code] ON [App].[LeaveRequisitionMaster] ([leave_requisition_master_employee_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_TaskTimeLogs_time_log_task_id] ON [App].[TaskTimeLogs] ([time_log_task_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    CREATE INDEX [IX_UserRole_user_role_role_id] ON [App].[UserRole] ([user_role_role_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260101124049_Initial_Tables'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260101124049_Initial_Tables', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102062037_alter_table_task_and_notifications'
)
BEGIN
    ALTER TABLE [App].[TaskTimeLogs] DROP CONSTRAINT [FK_TaskTimeLogs_Task_time_log_task_id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102062037_alter_table_task_and_notifications'
)
BEGIN
    DROP TABLE [App].[Task];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102062037_alter_table_task_and_notifications'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[Notification]') AND [c].[name] = N'notification_check_time');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [App].[Notification] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [App].[Notification] ALTER COLUMN [notification_check_time] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102062037_alter_table_task_and_notifications'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[Notification]') AND [c].[name] = N'notification_id');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [App].[Notification] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [App].[Notification] ALTER COLUMN [notification_id] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102062037_alter_table_task_and_notifications'
)
BEGIN
    CREATE TABLE [App].[UserTask] (
        [task_id] uniqueidentifier NOT NULL,
        [task_title] varchar(50) NOT NULL,
        [task_description] varchar(max) NOT NULL,
        [task_assigned_by] varchar(50) NOT NULL,
        [task_assigned_by_name] varchar(50) NULL,
        [task_assigned_to] varchar(50) NOT NULL,
        [task_assigned_to_name] varchar(50) NULL,
        [task_total_hours_worked] float NULL,
        [task_assign_date] datetime2 NOT NULL,
        [task_due_date] datetime2 NULL,
        [task_is_active] bit NOT NULL,
        [task_is_deleted] bit NULL,
        [task_priority] varchar(10) NOT NULL,
        [task_status] varchar(10) NOT NULL,
        CONSTRAINT [PK_UserTask] PRIMARY KEY ([task_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102062037_alter_table_task_and_notifications'
)
BEGIN
    ALTER TABLE [App].[TaskTimeLogs] ADD CONSTRAINT [FK_TaskTimeLogs_UserTask_time_log_task_id] FOREIGN KEY ([time_log_task_id]) REFERENCES [App].[UserTask] ([task_id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102062037_alter_table_task_and_notifications'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260102062037_alter_table_task_and_notifications', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102095816_alter_table_notification'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[UserTask]') AND [c].[name] = N'task_description');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [App].[UserTask] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [App].[UserTask] ALTER COLUMN [task_description] varchar(200) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102095816_alter_table_notification'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260102095816_alter_table_notification', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102100558_alter_table_notification_column_userCode'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[Notification]') AND [c].[name] = N'notification_user_code');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [App].[Notification] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [App].[Notification] ALTER COLUMN [notification_user_code] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102100558_alter_table_notification_column_userCode'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260102100558_alter_table_notification_column_userCode', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102101102_notification_table_altered'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[Notification]') AND [c].[name] = N'notification_user_code');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [App].[Notification] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [App].[Notification] ALTER COLUMN [notification_user_code] varchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102101102_notification_table_altered'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260102101102_notification_table_altered', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102103410_alter_table_tasks'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[UserTask]') AND [c].[name] = N'task_status');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [App].[UserTask] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [App].[UserTask] ALTER COLUMN [task_status] varchar(20) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102103410_alter_table_tasks'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[UserTask]') AND [c].[name] = N'task_priority');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [App].[UserTask] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [App].[UserTask] ALTER COLUMN [task_priority] varchar(20) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102103410_alter_table_tasks'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260102103410_alter_table_tasks', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102105709_alter_table_attendance_record'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[AttendanceRecord]') AND [c].[name] = N'att_rec_check_out_source');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [App].[AttendanceRecord] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [App].[AttendanceRecord] ALTER COLUMN [att_rec_check_out_source] nvarchar(10) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102105709_alter_table_attendance_record'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[AttendanceRecord]') AND [c].[name] = N'att_rec_check_in_source');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [App].[AttendanceRecord] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [App].[AttendanceRecord] ALTER COLUMN [att_rec_check_in_source] nvarchar(10) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260102105709_alter_table_attendance_record'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260102105709_alter_table_attendance_record', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionBalance] DROP CONSTRAINT [FK_LeaveRequisitionBalance_LeaveRequisitionMaster_leave_requisition_balance_master_id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionDetail] DROP CONSTRAINT [FK_LeaveRequisitionDetail_LeaveRequisitionMaster_leave_requisition_detail_master_id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionMaster] DROP CONSTRAINT [PK_LeaveRequisitionMaster];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionDetail] DROP CONSTRAINT [PK_LeaveRequisitionDetail];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    DROP INDEX [IX_LeaveRequisitionDetail_leave_requisition_detail_master_id] ON [App].[LeaveRequisitionDetail];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionBalance] DROP CONSTRAINT [PK_LeaveRequisitionBalance];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    DROP INDEX [IX_LeaveRequisitionBalance_leave_requisition_balance_master_id] ON [App].[LeaveRequisitionBalance];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[LeaveRequisitionMaster]') AND [c].[name] = N'leave_requisition_master_id');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [App].[LeaveRequisitionMaster] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [App].[LeaveRequisitionMaster] DROP COLUMN [leave_requisition_master_id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[LeaveRequisitionDetail]') AND [c].[name] = N'leave_requisition_detail_id');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [App].[LeaveRequisitionDetail] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [App].[LeaveRequisitionDetail] DROP COLUMN [leave_requisition_detail_id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[LeaveRequisitionBalance]') AND [c].[name] = N'leave_requisition_balance_id');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [App].[LeaveRequisitionBalance] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [App].[LeaveRequisitionBalance] DROP COLUMN [leave_requisition_balance_id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionMaster] ADD [Code] int NOT NULL IDENTITY;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionDetail] ADD [Code] int NOT NULL IDENTITY;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionBalance] ADD [Code] int NOT NULL IDENTITY;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionMaster] ADD CONSTRAINT [PK_LeaveRequisitionMaster] PRIMARY KEY ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionDetail] ADD CONSTRAINT [PK_LeaveRequisitionDetail] PRIMARY KEY ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionBalance] ADD CONSTRAINT [PK_LeaveRequisitionBalance] PRIMARY KEY ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075545_changepkinleavemaster'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260105075545_changepkinleavemaster', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionMaster] DROP CONSTRAINT [PK_LeaveRequisitionMaster];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionDetail] DROP CONSTRAINT [PK_LeaveRequisitionDetail];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionBalance] DROP CONSTRAINT [PK_LeaveRequisitionBalance];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[LeaveRequisitionMaster]') AND [c].[name] = N'Code');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [App].[LeaveRequisitionMaster] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [App].[LeaveRequisitionMaster] DROP COLUMN [Code];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[LeaveRequisitionDetail]') AND [c].[name] = N'Code');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [App].[LeaveRequisitionDetail] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [App].[LeaveRequisitionDetail] DROP COLUMN [Code];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[LeaveRequisitionBalance]') AND [c].[name] = N'Code');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [App].[LeaveRequisitionBalance] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [App].[LeaveRequisitionBalance] DROP COLUMN [Code];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionMaster] ADD [leave_requisition_master_id] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionDetail] ADD [leave_requisition_detail_id] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionBalance] ADD [leave_requisition_balance_id] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionMaster] ADD CONSTRAINT [PK_LeaveRequisitionMaster] PRIMARY KEY ([leave_requisition_master_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionDetail] ADD CONSTRAINT [PK_LeaveRequisitionDetail] PRIMARY KEY ([leave_requisition_detail_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionBalance] ADD CONSTRAINT [PK_LeaveRequisitionBalance] PRIMARY KEY ([leave_requisition_balance_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    CREATE INDEX [IX_LeaveRequisitionDetail_leave_requisition_detail_master_id] ON [App].[LeaveRequisitionDetail] ([leave_requisition_detail_master_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    CREATE INDEX [IX_LeaveRequisitionBalance_leave_requisition_balance_master_id] ON [App].[LeaveRequisitionBalance] ([leave_requisition_balance_master_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionBalance] ADD CONSTRAINT [FK_LeaveRequisitionBalance_LeaveRequisitionMaster_leave_requisition_balance_master_id] FOREIGN KEY ([leave_requisition_balance_master_id]) REFERENCES [App].[LeaveRequisitionMaster] ([leave_requisition_master_id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    ALTER TABLE [App].[LeaveRequisitionDetail] ADD CONSTRAINT [FK_LeaveRequisitionDetail_LeaveRequisitionMaster_leave_requisition_detail_master_id] FOREIGN KEY ([leave_requisition_detail_master_id]) REFERENCES [App].[LeaveRequisitionMaster] ([leave_requisition_master_id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105075940_removeAutoIncreamentleaveRequisitionMaster'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260105075940_removeAutoIncreamentleaveRequisitionMaster', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105082554_modifiedfieldsinleavedetail'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[LeaveRequisitionDetail]') AND [c].[name] = N'leave_requisition_detail_rejection_reason');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [App].[LeaveRequisitionDetail] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [App].[LeaveRequisitionDetail] ALTER COLUMN [leave_requisition_detail_rejection_reason] varchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105082554_modifiedfieldsinleavedetail'
)
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[LeaveRequisitionDetail]') AND [c].[name] = N'leave_requisition_detail_purpose');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [App].[LeaveRequisitionDetail] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [App].[LeaveRequisitionDetail] ALTER COLUMN [leave_requisition_detail_purpose] varchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105082554_modifiedfieldsinleavedetail'
)
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[LeaveRequisitionDetail]') AND [c].[name] = N'leave_requisition_detail_adjustment_type');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [App].[LeaveRequisitionDetail] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [App].[LeaveRequisitionDetail] ALTER COLUMN [leave_requisition_detail_adjustment_type] varchar(2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105082554_modifiedfieldsinleavedetail'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260105082554_modifiedfieldsinleavedetail', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105115817_editFkInAttendanceLeave'
)
BEGIN
    ALTER TABLE [App].[AttendanceLeave] DROP CONSTRAINT [FK_AttendanceLeave_AttendanceRecord_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_fortnight_att_leave_year];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105115817_editFkInAttendanceLeave'
)
BEGIN
    DROP INDEX [IX_AttendanceLeave_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_fortnight_att_leave_year] ON [App].[AttendanceLeave];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105115817_editFkInAttendanceLeave'
)
BEGIN
    ALTER TABLE [App].[AttendanceLeave] ADD CONSTRAINT [FK_AttendanceLeave_AttendanceRecord_att_leave_employee_code_att_leave_day_att_leave_month_att_leave_year_att_leave_fortnight] FOREIGN KEY ([att_leave_employee_code], [att_leave_day], [att_leave_month], [att_leave_year], [att_leave_fortnight]) REFERENCES [App].[AttendanceRecord] ([att_rec_employee_code], [att_rec_day], [att_rec_month], [att_rec_year], [att_rec_fortnight]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260105115817_editFkInAttendanceLeave'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260105115817_editFkInAttendanceLeave', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260113102641_create_table_Entity'
)
BEGIN
    CREATE TABLE [General].[Entity] (
        [entity_code] varchar(4) NOT NULL,
        [entity_name] nvarchar(100) NOT NULL,
        [entity_legal_name] nvarchar(100) NOT NULL,
        [entity_short_name] nvarchar(15) NOT NULL,
        [entity_country_code] varchar(10) NOT NULL,
        [entity_address] nvarchar(500) NULL,
        [entity_email] varchar(50) NULL,
        [entity_phone] varchar(50) NULL,
        [entity_industry_code] varchar(8) NULL,
        [entity_fax] nvarchar(30) NULL,
        [entity_gl_code] varchar(2) NULL,
        [entity_fiscal_year] nvarchar(20) NULL,
        [latitude] float NULL,
        [longitude] float NULL,
        [entity_parent_code] varchar(20) NULL,
        [entity_child_code] varchar(20) NULL,
        [entity_status] bit NOT NULL,
        [entity_client_vendor] bit NOT NULL,
        [entity_allowed_radius_meters] int NULL,
        CONSTRAINT [PK_Entity] PRIMARY KEY ([entity_code])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260113102641_create_table_Entity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260113102641_create_table_Entity', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260113103403_add_fields_in_task_table'
)
BEGIN
    ALTER TABLE [App].[UserTask] ADD [task_entity_code] varchar(4) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260113103403_add_fields_in_task_table'
)
BEGIN
    CREATE INDEX [IX_UserTask_task_entity_code] ON [App].[UserTask] ([task_entity_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260113103403_add_fields_in_task_table'
)
BEGIN
    ALTER TABLE [App].[UserTask] ADD CONSTRAINT [FK_UserTask_Entity_task_entity_code] FOREIGN KEY ([task_entity_code]) REFERENCES [General].[Entity] ([entity_code]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260113103403_add_fields_in_task_table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260113103403_add_fields_in_task_table', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260114063512_add_field_in_Employee_Table'
)
BEGIN
    ALTER TABLE [General].[Employee] ADD [employee_face_image] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260114063512_add_field_in_Employee_Table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260114063512_add_field_in_Employee_Table', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121050713_addtaskPreferanceColumninUserTask'
)
BEGIN
    ALTER TABLE [App].[UserTask] ADD [task_preferance] varchar(20) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121050713_addtaskPreferanceColumninUserTask'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260121050713_addtaskPreferanceColumninUserTask', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123115215_add_table_Settings.Configuration'
)
BEGIN
    CREATE TABLE [Settings].[Configuration] (
        [configuration_heading] varchar(20) NOT NULL,
        [configuration_title] varchar(30) NOT NULL,
        [configuration_value_type] varchar(7) NOT NULL,
        [configuration_value] varchar(1000) NOT NULL,
        CONSTRAINT [PK_Configuration] PRIMARY KEY ([configuration_heading], [configuration_title])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260123115215_add_table_Settings.Configuration'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260123115215_add_table_Settings.Configuration', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126045729_addColumnInEmpolyeeIsAppUser'
)
BEGIN
    ALTER TABLE [General].[Employee] ADD [employee_is_app_user] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260126045729_addColumnInEmpolyeeIsAppUser'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260126045729_addColumnInEmpolyeeIsAppUser', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127064144_addTableRegistrationImage'
)
BEGIN
    CREATE TABLE [App].[registration_image] (
        [registration_image_code] uniqueidentifier NOT NULL,
        [registration_image_employee_code] varchar(20) NOT NULL,
        [registration_imaget_admin_code] nvarchar(20) NOT NULL,
        [registration_image_request_date] datetime2 NOT NULL,
        [registration_image_approve_date] datetime2 NULL,
        [registration_image_rejection_date] datetime2 NULL,
        [registration_image_rejection_reason] varchar(250) NULL,
        [registration_image_face_image] varchar(max) NOT NULL,
        [registration_image_status] bit NOT NULL,
        [registration_image_history_status] bit NOT NULL,
        [registration_image_company_code] nvarchar(8) NOT NULL,
        CONSTRAINT [PK_registration_image] PRIMARY KEY ([registration_image_code]),
        CONSTRAINT [FK_registration_image_Employee_registration_image_employee_code] FOREIGN KEY ([registration_image_employee_code]) REFERENCES [General].[Employee] ([employee_code]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127064144_addTableRegistrationImage'
)
BEGIN
    CREATE INDEX [IX_registration_image_registration_image_employee_code] ON [App].[registration_image] ([registration_image_employee_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260127064144_addTableRegistrationImage'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260127064144_addTableRegistrationImage', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128051020_add_col_isActive_in_device_table'
)
BEGIN
    ALTER TABLE [General].[Devices] ADD [device_is_active] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128051020_add_col_isActive_in_device_table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260128051020_add_col_isActive_in_device_table', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128115020_add_col_CompanyCode_in_device_table'
)
BEGIN
    ALTER TABLE [General].[Devices] ADD [device_company_code] nvarchar(4) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128115020_add_col_CompanyCode_in_device_table'
)
BEGIN
    CREATE INDEX [IX_Devices_device_company_code] ON [General].[Devices] ([device_company_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128115020_add_col_CompanyCode_in_device_table'
)
BEGIN
    ALTER TABLE [General].[Devices] ADD CONSTRAINT [FK_Devices_Company_device_company_code] FOREIGN KEY ([device_company_code]) REFERENCES [General].[Company] ([company_code]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128115020_add_col_CompanyCode_in_device_table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260128115020_add_col_CompanyCode_in_device_table', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128115125_alter_col_companycode_in_device_table'
)
BEGIN
    DROP INDEX [IX_Devices_device_company_code] ON [General].[Devices];
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[General].[Devices]') AND [c].[name] = N'device_company_code');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [General].[Devices] DROP CONSTRAINT [' + @var18 + '];');
    EXEC(N'UPDATE [General].[Devices] SET [device_company_code] = N'''' WHERE [device_company_code] IS NULL');
    ALTER TABLE [General].[Devices] ALTER COLUMN [device_company_code] nvarchar(4) NOT NULL;
    ALTER TABLE [General].[Devices] ADD DEFAULT N'' FOR [device_company_code];
    CREATE INDEX [IX_Devices_device_company_code] ON [General].[Devices] ([device_company_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260128115125_alter_col_companycode_in_device_table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260128115125_alter_col_companycode_in_device_table', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129094135_edit_col_isDeleted_in_task_table'
)
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[UserTask]') AND [c].[name] = N'task_is_deleted');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [App].[UserTask] DROP CONSTRAINT [' + @var19 + '];');
    EXEC(N'UPDATE [App].[UserTask] SET [task_is_deleted] = 0 WHERE [task_is_deleted] IS NULL');
    ALTER TABLE [App].[UserTask] ALTER COLUMN [task_is_deleted] bit NOT NULL;
    ALTER TABLE [App].[UserTask] ADD DEFAULT (0) FOR [task_is_deleted];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129094135_edit_col_isDeleted_in_task_table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260129094135_edit_col_isDeleted_in_task_table', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129102745_addDeleteFlagInDevicetable'
)
BEGIN
    ALTER TABLE [General].[Devices] ADD [device_deleted_at] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129102745_addDeleteFlagInDevicetable'
)
BEGIN
    ALTER TABLE [General].[Devices] ADD [device_is_delete] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129102745_addDeleteFlagInDevicetable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260129102745_addDeleteFlagInDevicetable', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129103050_Add_Table_CancelLeave'
)
BEGIN
    CREATE TABLE [App].[CancelLeave] (
        [cancel_leave_Id] uniqueidentifier NOT NULL,
        [cancel_leave_employee_code] varchar(20) NOT NULL,
        [cancel_leave_year] int NOT NULL,
        [cancel_leave_month] int NOT NULL,
        [cancel_leave_day] int NOT NULL,
        [cancel_leave_date] datetime2 NOT NULL,
        [cancel_leave_approver_employee_code] varchar(20) NOT NULL,
        [cancel_leave_purpose] varchar(200) NULL,
        [cancel_leave_state] varchar(5) NOT NULL,
        [cancel_leave_approve_date] datetime2 NULL,
        CONSTRAINT [PK_CancelLeave] PRIMARY KEY ([cancel_leave_Id], [cancel_leave_employee_code], [cancel_leave_year], [cancel_leave_month], [cancel_leave_day])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129103050_Add_Table_CancelLeave'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260129103050_Add_Table_CancelLeave', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129113909_add_col_DaletedAt_and_UpdatedAt_in_task_table'
)
BEGIN
    ALTER TABLE [App].[UserTask] ADD [task_updated_at] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129113909_add_col_DaletedAt_and_UpdatedAt_in_task_table'
)
BEGIN
    ALTER TABLE [App].[UserTask] ADD [task_deleted_at] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260129113909_add_col_DaletedAt_and_UpdatedAt_in_task_table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260129113909_add_col_DaletedAt_and_UpdatedAt_in_task_table', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[AttendanceRecord]') AND [c].[name] = N'att_rec_device_id');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [App].[AttendanceRecord] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [App].[AttendanceRecord] DROP COLUMN [att_rec_device_id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[AttendanceRecord]') AND [c].[name] = N'att_rec_device_name');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [App].[AttendanceRecord] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [App].[AttendanceRecord] DROP COLUMN [att_rec_device_name];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[AttendanceRecord]') AND [c].[name] = N'att_rec_location');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [App].[AttendanceRecord] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [App].[AttendanceRecord] DROP COLUMN [att_rec_location];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    ALTER TABLE [General].[Devices] DROP CONSTRAINT [PK_Devices];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[General].[Devices]') AND [c].[name] = N'device_id');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [General].[Devices] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [General].[Devices] ALTER COLUMN [device_id] uniqueidentifier NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    ALTER TABLE [General].[Devices] ADD CONSTRAINT [PK_Devices] PRIMARY KEY ([device_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    ALTER TABLE [App].[AttendanceRecord] ADD [att_rec_check_in_device_id] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    ALTER TABLE [App].[AttendanceRecord] ADD [att_rec_check_out_device_id] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    CREATE INDEX [IX_AttendanceRecord_att_rec_check_in_device_id] ON [App].[AttendanceRecord] ([att_rec_check_in_device_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    CREATE INDEX [IX_AttendanceRecord_att_rec_check_out_device_id] ON [App].[AttendanceRecord] ([att_rec_check_out_device_id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    ALTER TABLE [App].[AttendanceRecord] ADD CONSTRAINT [FK_AttendanceRecord_Devices_att_rec_check_in_device_id] FOREIGN KEY ([att_rec_check_in_device_id]) REFERENCES [General].[Devices] ([device_id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    ALTER TABLE [App].[AttendanceRecord] ADD CONSTRAINT [FK_AttendanceRecord_Devices_att_rec_check_out_device_id] FOREIGN KEY ([att_rec_check_out_device_id]) REFERENCES [General].[Devices] ([device_id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130071205_alter_table_device_and_attendancerecord'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260130071205_alter_table_device_and_attendancerecord', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130105107_changesInCancelLeaveTable'
)
BEGIN
    ALTER TABLE [App].[CancelLeave] DROP CONSTRAINT [PK_CancelLeave];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130105107_changesInCancelLeaveTable'
)
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[CancelLeave]') AND [c].[name] = N'cancel_leave_year');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [App].[CancelLeave] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [App].[CancelLeave] DROP COLUMN [cancel_leave_year];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130105107_changesInCancelLeaveTable'
)
BEGIN
    DECLARE @var25 sysname;
    SELECT @var25 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[CancelLeave]') AND [c].[name] = N'cancel_leave_month');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [App].[CancelLeave] DROP CONSTRAINT [' + @var25 + '];');
    ALTER TABLE [App].[CancelLeave] DROP COLUMN [cancel_leave_month];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130105107_changesInCancelLeaveTable'
)
BEGIN
    DECLARE @var26 sysname;
    SELECT @var26 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[CancelLeave]') AND [c].[name] = N'cancel_leave_day');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [App].[CancelLeave] DROP CONSTRAINT [' + @var26 + '];');
    ALTER TABLE [App].[CancelLeave] DROP COLUMN [cancel_leave_day];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130105107_changesInCancelLeaveTable'
)
BEGIN
    DECLARE @var27 sysname;
    SELECT @var27 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[CancelLeave]') AND [c].[name] = N'cancel_leave_date');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [App].[CancelLeave] DROP CONSTRAINT [' + @var27 + '];');
    ALTER TABLE [App].[CancelLeave] DROP COLUMN [cancel_leave_date];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130105107_changesInCancelLeaveTable'
)
BEGIN
    ALTER TABLE [App].[CancelLeave] ADD [cancel_leave_from_date] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130105107_changesInCancelLeaveTable'
)
BEGIN
    ALTER TABLE [App].[CancelLeave] ADD [cancel_leave_to_date] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130105107_changesInCancelLeaveTable'
)
BEGIN
    ALTER TABLE [App].[CancelLeave] ADD CONSTRAINT [PK_CancelLeave] PRIMARY KEY ([cancel_leave_Id], [cancel_leave_employee_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260130105107_changesInCancelLeaveTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260130105107_changesInCancelLeaveTable', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260202053343_add_reject_columns'
)
BEGIN
    ALTER TABLE [App].[CancelLeave] ADD [cancel_leave_reject_at] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260202053343_add_reject_columns'
)
BEGIN
    ALTER TABLE [App].[CancelLeave] ADD [cancel_leave_reject_reason] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260202053343_add_reject_columns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260202053343_add_reject_columns', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260202075527_addcolumnincancelleave'
)
BEGIN
    ALTER TABLE [App].[CancelLeave] ADD [cancel_leave_submission_date] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260202075527_addcolumnincancelleave'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260202075527_addcolumnincancelleave', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206072143_add_col_CanReadLogs_in_Device_table'
)
BEGIN
    ALTER TABLE [General].[Devices] ADD [device_canreadlogs] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206072143_add_col_CanReadLogs_in_Device_table'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260206072143_add_col_CanReadLogs_in_Device_table', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206123700_addColumnExtrahour'
)
BEGIN
    ALTER TABLE [App].[TaskTimeLogs] ADD [is_extra_work] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260206123700_addColumnExtrahour'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260206123700_addColumnExtrahour', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260212073537_AdjustCancelLeaveTable'
)
BEGIN
    DECLARE @var28 sysname;
    SELECT @var28 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[App].[CancelLeave]') AND [c].[name] = N'cancel_leave_from_date');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [App].[CancelLeave] DROP CONSTRAINT [' + @var28 + '];');
    ALTER TABLE [App].[CancelLeave] DROP COLUMN [cancel_leave_from_date];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260212073537_AdjustCancelLeaveTable'
)
BEGIN
    EXEC sp_rename N'[App].[CancelLeave].[cancel_leave_to_date]', N'leave_date', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260212073537_AdjustCancelLeaveTable'
)
BEGIN
    ALTER TABLE [App].[CancelLeave] ADD [LeaveId] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260212073537_AdjustCancelLeaveTable'
)
BEGIN
    CREATE INDEX [IX_CancelLeave_LeaveId] ON [App].[CancelLeave] ([LeaveId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260212073537_AdjustCancelLeaveTable'
)
BEGIN
    ALTER TABLE [App].[CancelLeave] ADD CONSTRAINT [FK_CancelLeave_LeaveRequisitionMaster_LeaveId] FOREIGN KEY ([LeaveId]) REFERENCES [App].[LeaveRequisitionMaster] ([leave_requisition_master_id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260212073537_AdjustCancelLeaveTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260212073537_AdjustCancelLeaveTable', N'9.0.11');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260216083058_add_table_FaceImages'
)
BEGIN
    EXEC sp_rename N'[General].[Employee].[employee_face_image]', N'employee_face_embedding', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260216083058_add_table_FaceImages'
)
BEGIN
    CREATE TABLE [App].[FaceImages] (
        [face_image_id] uniqueidentifier NOT NULL,
        [employee_code] varchar(20) NOT NULL,
        [face_image] nvarchar(max) NULL,
        CONSTRAINT [PK_FaceImages] PRIMARY KEY ([face_image_id], [employee_code]),
        CONSTRAINT [FK_FaceImages_Employee_employee_code] FOREIGN KEY ([employee_code]) REFERENCES [General].[Employee] ([employee_code]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260216083058_add_table_FaceImages'
)
BEGIN
    CREATE UNIQUE INDEX [IX_FaceImages_employee_code] ON [App].[FaceImages] ([employee_code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260216083058_add_table_FaceImages'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260216083058_add_table_FaceImages', N'9.0.11');
END;

COMMIT;
GO

