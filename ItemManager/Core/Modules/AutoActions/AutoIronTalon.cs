﻿namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    [AbilityBasedModule(AbilityId.item_iron_talon)]
    internal class AutoIronTalon : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly IronTalonMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private IronTalon ironTalon;

        public AutoIronTalon(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.IronTalonMenu;

            Refresh();

            Game.OnUpdate += OnUpdate;
        }

        public List<AbilityId> AbilityIds { get; } = new List<AbilityId>
        {
            AbilityId.item_iron_talon
        };

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
        }

        public void Refresh()
        {
            ironTalon = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityIds.First()) as IronTalon;
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(500);

            if (!menu.IsEnabled || !manager.MyHero.CanUseItems() || !ironTalon.CanBeCasted() || Game.IsPaused)
            {
                return;
            }

            var creep = ObjectManager.GetEntitiesParallel<Creep>()
                .Where(
                    x => x.IsValid && x.IsAlive && x.IsSpawned && x.IsVisible
                         && x.Distance2D(manager.MyHero.Position) <= ironTalon.GetCastRange()
                         && x.Team != manager.MyHero.Team && !x.IsAncient
                         && x.Health * ironTalon.Damage >= menu.DamageThreshold)
                .OrderByDescending(x => x.Health)
                .FirstOrDefault();

            if (creep != null)
            {
                ironTalon.Use(creep);
            }
        }
    }
}