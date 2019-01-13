#region Copyright & License Information
/*
 * Copyright 2007-2018 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System.Drawing;
using OpenRA.Activities;
using OpenRA.Mods.Common.Traits;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Activities
{
	public class FlyFollow : Activity
	{
		readonly Aircraft aircraft;
		readonly WDist minRange;
		readonly WDist maxRange;
		readonly Color? targetLineColor;
		Target target;

		public FlyFollow(Actor self, Target target, WDist minRange, WDist maxRange, Color? targetLineColor = null)
		{
			this.target = target;
			aircraft = self.Trait<Aircraft>();
			this.minRange = minRange;
			this.maxRange = maxRange;
			this.targetLineColor = targetLineColor;
		}

		public override Activity Tick(Actor self)
		{
			// Refuse to take off if it would land immediately again.
			if (aircraft.ForceLanding)
			{
				Cancel(self);
				return NextActivity;
			}

			if (IsCanceled || !target.IsValidFor(self))
				return NextActivity;

			if (target.IsInRange(self.CenterPosition, maxRange) && !target.IsInRange(self.CenterPosition, minRange))
			{
				Fly.FlyToward(self, aircraft, aircraft.Facing, aircraft.Info.CruiseAltitude);
				return this;
			}

			return ActivityUtils.SequenceActivities(new Fly(self, target, minRange, maxRange, targetLineColor: targetLineColor), this);
		}
	}
}
