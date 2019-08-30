using ForumSiteCore.Business.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.Business.Enums
{
    public static class EnumTranslator
    {
        public static VoteType DirectionToVoteType(Boolean? direction)
        {
            switch(direction)
            {
                case true:
                    return VoteType.Up;
                case false:
                    return VoteType.Down;
                case null:
                    return VoteType.None;
                default:
                    throw new Exception("Unidentified direction");
            }
        }

        public static Boolean? VoteTypeToDirection(VoteType voteType)
        {
            switch (voteType)
            {
                case VoteType.Up:
                    return true;
                case VoteType.Down:
                    return false;
                case VoteType.None:
                    return null;
                default:
                    throw new Exception("Unidentified VoteType");
            }
        }

    }
}
