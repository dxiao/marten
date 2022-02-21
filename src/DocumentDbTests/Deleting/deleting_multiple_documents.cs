﻿using Marten;
using Marten.Testing.Documents;
using Marten.Testing.Harness;
using Shouldly;
using Xunit;

namespace DocumentDbTests.Deleting
{

    public class deleting_multiple_documents : IntegrationContext
    {
        [Theory]
        [SessionTypes]
        public void multiple_documents(DocumentTracking tracking)
        {
            DocumentTracking = tracking;

            #region sample_mixed-docs-to-store
            var user1 = new User {FirstName = "Jeremy", LastName = "Miller"};
            var issue1 = new Issue {Title = "TV won't turn on"}; // unfortunately true as I write this...
            var company1 = new Company{Name = "Widgets, inc."};
            var company2 = new Company{Name = "BigCo"};
            var company3 = new Company{Name = "SmallCo"};

            theSession.Store<object>(user1, issue1, company1, company2, company3);
            #endregion

            theSession.SaveChanges();

            using (var session = theStore.OpenSession())
            {
                var user = session.Load<User>(user1.Id);
                user.FirstName = "Max";

                session.Store(user);

                session.Delete(company2);

                session.SaveChanges();
            }

            using (var session = theStore.QuerySession())
            {
                session.Load<User>(user1.Id).FirstName.ShouldBe("Max");
                session.Load<Company>(company1.Id).Name.ShouldBe("Widgets, inc.");
                session.Load<Company>(company2.Id).ShouldBeNull();
                session.Load<Company>(company3.Id).Name.ShouldBe("SmallCo");
            }
        }

        public deleting_multiple_documents(DefaultStoreFixture fixture) : base(fixture)
        {
        }
    }
}