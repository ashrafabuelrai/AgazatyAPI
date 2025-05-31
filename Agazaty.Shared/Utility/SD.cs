using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agazaty.Shared.Utility
{
    public static class SD
    {
        public enum CountsFromNormalLeaveTypes
        {
            FromNormalLeave,
            From81Before3Years,
            From81Before2Years,
            From81Before1Years,
            From47
        }
        public enum Holder
        {
            CoWorker,
            DirectManager,
            GeneralManager,
            NotWaiting
        }

        public enum LeaveStatus
        {
            Waiting,
            Accepted,
            Rejected
        }

        public static class LeaveTypes
        {
            public enum Leaves
            {
                اعتيادية,
                عارضة,
                مرضية
            }

            public static readonly List<string> res = Enum.GetNames(typeof(Leaves)).ToList();
        }

        public enum NormalLeaveSection
        {
            NoSection,
            SixMonths,
            OneYear,
            TenYears,
            FiftyAge,
            DisabilityEmployee
        }

        public enum RejectedBy
        {
            CoWorker,
            DirectManager,
            GeneralManager,
            NotRejected
        }
    }
}
