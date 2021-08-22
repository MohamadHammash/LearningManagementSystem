using System;
using TechTalk.SpecFlow;

namespace SpecFlowProject1.Steps
{
    [Binding]
    public class SpecFlowFeature1Steps
    {
        [Given(@"that the user has the role of admin")]
        public void GivenThatTheUserHasTheRoleOfAdmin()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"when the user logs in")]
        public void WhenWhenTheUserLogsIn()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the AdminOverview view should be displayed")]
        public void ThenTheAdminOverviewViewShouldBeDisplayed()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
