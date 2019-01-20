using ForumSiteCore.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Enums
{
    public static class EnumTranslator
    {
        public static Boolean? VoteTypeToDirection(VoteType voteType)
        {
            switch (voteType)
            {
                case VoteType.Up:
                    return true;
                case VoteType.Down:
                    return false;
                default:
                    return null;
            }
        }

    }
}
