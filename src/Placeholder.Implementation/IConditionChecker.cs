﻿using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation
{
   public interface IConditionChecker
   {
     ConditionValidationType Validate(string stubId, StubConditionsModel conditions);
   }
}
