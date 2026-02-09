using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_Project.Models.Enums
{
    public enum Designation
    {
        // Entry Level
        Intern = 1,
        TraineeEngineer = 2,
        JuniorDeveloper = 3,

        // Development Roles
        Developer = 4,
        SeniorDeveloper = 5,
        LeadDeveloper = 6,
        PrincipalEngineer = 7,
        SoftwareArchitect = 8,
        SolutionArchitect = 9,

        // Testing / QA Roles
        QAEngineer = 10,
        SeniorQAEngineer = 11,
        AutomationTestEngineer = 12,
        PerformanceTestEngineer = 13,
        QALead = 14,

        // DevOps Roles
        DevOpsEngineer = 15,
        SeniorDevOpsEngineer = 16,
        SiteReliabilityEngineer = 17,
        CloudEngineer = 18,
        ReleaseManager = 19,

        // Management Roles
        TeamLead = 20,
        ProjectManager = 21,
        EngineeringManager = 22,
        DeliveryManager = 23,
        TechnicalManager = 24,

        // HR / Support
        HR = 25,
        Recruiter = 26,
        ITSupportEngineer = 27
    }

}
