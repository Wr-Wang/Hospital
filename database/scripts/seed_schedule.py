import subprocess
from datetime import date, timedelta

doctors = [
    (2, 2, 1),   # 张医生 - 内科
    (6, 2, 1),   # 刘洋 - 内科
    (10, 2, 1),  # 周磊 - 内科
    (8, 3, 1),   # 王强 - 外科
    (9, 3, 1),   # 黄勇 - 外科
]

today = date(2026, 5, 25)
batches = []

for d in range(1, 8):
    dt = today + timedelta(days=d)
    date_str = dt.strftime("%Y-%m-%d")
    for doc_id, dept_id, campus_id in doctors:
        batches.append(f"""DECLARE @tid bigint;
INSERT INTO opd.ScheduleTemplates (DoctorId, DepartmentId, CampusId, ScheduleDate, Status) VALUES ({doc_id}, {dept_id}, {campus_id}, '{date_str}', N'已发布');
SET @tid = SCOPE_IDENTITY();
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'上午', '08:00', '12:00', 30, 0, @tid);
INSERT INTO opd.ScheduleSlots (SlotType, StartTime, EndTime, TotalQuota, BookedQuota, TemplateId) VALUES (N'下午', '14:00', '17:00', 20, 0, @tid);
GO""")

full_sql = "\n".join(batches)
full_sql += "\nSELECT COUNT(*) AS TotalTemplates FROM opd.ScheduleTemplates WHERE ScheduleDate > '2026-05-25';"

with open("e:\\Demo\\Cursor\\Hospital\\seed_schedule_final.sql", "w", encoding="utf-8-sig") as f:
    f.write(full_sql)

print(f"Generated {len(batches)} batches")

result = subprocess.run(
    ["sqlcmd", "-S", ".", "-d", "Hospital", "-U", "sa", "-P", "123456", "-C",
     "-i", "e:\\Demo\\Cursor\\Hospital\\seed_schedule_final.sql"],
    capture_output=True, text=True, encoding="gbk"
)
# sqlcmd output is in gbk encoding
print(result.stdout[:2000] if result.stdout else "No output")
if result.stderr:
    stderr_gbk = result.stderr.encode("latin1").decode("gbk", errors="replace")
    print("STDERR:", stderr_gbk[:500])
