using System;

namespace $ext_projectname$.Domain.Types
{
    public enum QueueTypes
    {
        Classic,
        Quorum
    }

    [Flags]
    public enum AffiliationTypes
    {
        None = 0,
        Student = 1,
        EduPersonAffiliated = 2,
        GaScopedAffiliated = 4,
        UncStudentTypeAffiliated = 8,
        Employee = 16, // 0x00000010
        StudentAffiliated = UncStudentTypeAffiliated | Student, // 0x00000009
        AllOtherAffiliated = UncStudentTypeAffiliated | GaScopedAffiliated | EduPersonAffiliated, // 0x0000000E
    }
    
    


}
