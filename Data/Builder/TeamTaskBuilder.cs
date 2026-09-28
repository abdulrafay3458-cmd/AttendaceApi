using AttendanceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data.Builder
{
    public class TeamTaskBuilder
    {
        public static void BuildTeamTask(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<TeamTask>(entity =>
            {
                entity.ToTable("TeamTask", "App");

                entity.HasKey(x => new
                {
                    x.TeamTaskId,
                    x.TaskId
                });

                // Relationship with UserTask
                entity.HasOne(x => x.Task)
                    .WithMany(x => x.TeamTasks)
                    .HasForeignKey(x => x.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
