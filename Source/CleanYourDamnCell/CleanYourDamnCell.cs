using RimWorld;
using Verse;
using Verse.AI;

namespace CleanYourDamnCell
{
    public class JobGiver_CleanCell : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Room room = pawn.GetRoom();
            if (room == null || !room.IsPrisonCell)
            {
                return null;
            }
            Map map = pawn.Map;
            Job job = JobMaker.MakeJob(JobDefOf.Clean);
            foreach (IntVec3 cell in room.Cells)
            {
                foreach (Thing thing in cell.GetThingList(map))
                {
                    if (HasJobOnThing(pawn, thing))
                    {
                        job.AddQueuedTarget(TargetIndex.A, thing);
                    }
                }
                if (job.GetTargetQueue(TargetIndex.A).Count >= 15)
                {
                    break;
                }
            }
            if (job.targetQueueA == null || job.targetQueueA.Count < 1)
            {
                return null;
            }
            job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
            return job;
        }

        private bool HasJobOnThing(Pawn pawn, Thing t)
        {
            if (!(t is Filth filth))
            {
                return false;
            }
            if (!filth.Map.areaManager.Home[filth.Position])
            {
                return false;
            }
            if (!pawn.CanReserve(t))
            {
                return false;
            }
            if (filth.TicksSinceThickened < 600)
            {
                return false;
            }
            return true;
        }
    }
}