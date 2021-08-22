using System;
using TechTalk.SpecFlow;

namespace LmsSpecFlowTest.Steps
{
    [Binding]
    public class LandingPageSteps
    {
        public ScenarioContext scenarioContext { get; set; }
        LandingPageSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given(@"that the user is an admin")]
        public void GivenThatTheUserIsAnAdmin()
        {
            scenarioContext.Pending();
        }
        
        [When(@"the user logs in")]
        public void WhenTheUserLogsIn()
        {
            scenarioContext.Pending();
        }
        
        [Then(@"the user should be redirected to AdminOverview view")]
        public void ThenTheUserShouldBeRedirectedToAdminOverviewView()
        {
            scenarioContext.Pending();
        }
    }
}
