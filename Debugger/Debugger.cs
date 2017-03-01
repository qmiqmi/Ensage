﻿namespace Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using Menus;

    using Color = System.ConsoleColor;

    internal class Debugger
    {
        #region Fields

        private Logger logger;

        private MenuManager mainMenu;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            Unit.OnModifierAdded -= OnModifierAdded;
            Unit.OnModifierRemoved -= OnModifierRemoved;
            Entity.OnParticleEffectAdded -= OnParticleEffectAdded;
            ObjectManager.OnAddEntity -= OnEntityAdded;
            ObjectManager.OnRemoveEntity -= OnEntityRemoved;
            ObjectManager.OnAddTrackingProjectile -= OnTrackingProjectileAdded;
            ObjectManager.OnRemoveTrackingProjectile -= OnTrackingProjectileRemoved;

            Entity.OnAnimationChanged -= OnAnimationChanged;
            Entity.OnBoolPropertyChange -= OnBoolPropertyChanged;
            Entity.OnFloatPropertyChange -= OnFloatPropertyChanged;
            Entity.OnHandlePropertyChange -= OnHandlePropertyChanged;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChanged;
            Entity.OnInt64PropertyChange -= OnInt64PropertyChanged;
            Entity.OnStringPropertyChange -= OnStringPropertyChanged;

            Player.OnExecuteOrder -= OnExecuteOrder;

            mainMenu.DumpingMenu.Spells.OnDump -= SpellsOnDump;
            mainMenu.DumpingMenu.Items.OnDump -= ItemsOnDump;
            mainMenu.DumpingMenu.Modifiers.OnDump -= ModifiersOnDump;
            mainMenu.DumpingMenu.Units.OnDump -= UnitsOnDump;

            mainMenu.OnClose();
        }

        public void OnLoad()
        {
            mainMenu = new MenuManager();
            logger = new Logger();

            Unit.OnModifierAdded += OnModifierAdded;
            Unit.OnModifierRemoved += OnModifierRemoved;
            Entity.OnParticleEffectAdded += OnParticleEffectAdded;
            ObjectManager.OnAddEntity += OnEntityAdded;
            ObjectManager.OnRemoveEntity += OnEntityRemoved;
            ObjectManager.OnAddTrackingProjectile += OnTrackingProjectileAdded;
            ObjectManager.OnRemoveTrackingProjectile += OnTrackingProjectileRemoved;

            Entity.OnAnimationChanged += OnAnimationChanged;
            Entity.OnBoolPropertyChange += OnBoolPropertyChanged;
            Entity.OnFloatPropertyChange += OnFloatPropertyChanged;
            Entity.OnHandlePropertyChange += OnHandlePropertyChanged;
            Entity.OnInt32PropertyChange += OnInt32PropertyChanged;
            Entity.OnInt64PropertyChange += OnInt64PropertyChanged;
            Entity.OnStringPropertyChange += OnStringPropertyChanged;

            Player.OnExecuteOrder += OnExecuteOrder;

            mainMenu.DumpingMenu.Spells.OnDump += SpellsOnDump;
            mainMenu.DumpingMenu.Items.OnDump += ItemsOnDump;
            mainMenu.DumpingMenu.Modifiers.OnDump += ModifiersOnDump;
            mainMenu.DumpingMenu.Units.OnDump += UnitsOnDump;
        }

        #endregion

        #region Methods

        private void ItemsOnDump(object sender, EventArgs e)
        {
            var unit = ObjectManager.LocalPlayer.Selection.FirstOrDefault() as Unit;

            if (unit == null || !unit.IsValid)
            {
                return;
            }

            var menu = mainMenu.DumpingMenu.Items;

            const Color Color = Color.Yellow;
            const Logger.Type Type = Logger.Type.Item;

            logger.Write("Item dump", Type, Color, true);
            logger.Write("Unit name: " + unit.Name, Type, Color);
            logger.Write("Unit classID: " + unit.ClassID, Type, Color);
            logger.EmptyLine();

            if (!unit.HasInventory)
            {
                return;
            }

            var items = new List<Item>();

            if (menu.ShowInventoryItems)
            {
                items.AddRange(unit.Inventory.Items);
            }
            if (menu.ShowBackpackItems)
            {
                items.AddRange(unit.Inventory.Backpack);
            }
            if (menu.ShowStashItems)
            {
                items.AddRange(unit.Inventory.Stash);
            }

            foreach (var item in items)
            {
                logger.Write("Name: " + item.Name, Type, Color);
                logger.Write("ClassID: " + item.ClassID, Type, Color);

                if (menu.ShowManaCost)
                {
                    logger.Write("Mana cost: " + item.ManaCost, Type, Color);
                }
                if (menu.ShowCastRange)
                {
                    logger.Write("Cast range: " + item.GetCastRange(), Type, Color);
                }
                if (menu.ShowBehavior)
                {
                    logger.Write("Behavior: " + item.AbilityBehavior, Type, Color);
                }
                if (menu.ShowTargetType)
                {
                    logger.Write("Target type: " + item.TargetType, Type, Color);
                    logger.Write("Target team type: " + item.TargetTeamType, Type, Color);
                }
                if (menu.ShowSpecialData)
                {
                    logger.Write("Special data =>", Type, Color);
                    foreach (var abilitySpecialData in item.AbilitySpecialData)
                    {
                        logger.Write("  " + abilitySpecialData.Name + ": " + abilitySpecialData.Value, Type, Color);
                    }
                }

                logger.EmptyLine();
            }

            logger.EmptyLine();
        }

        private void ModifiersOnDump(object sender, EventArgs e)
        {
            var unit = ObjectManager.LocalPlayer.Selection.FirstOrDefault() as Unit;

            if (unit == null || !unit.IsValid)
            {
                return;
            }

            var menu = mainMenu.DumpingMenu.Modifiers;

            const Color Color = Color.Yellow;
            const Logger.Type Type = Logger.Type.Modifier;

            logger.Write("Modifier dump", Type, Color, true);
            logger.Write("Unit name: " + unit.Name, Type, Color);
            logger.Write("Unit classID: " + unit.ClassID, Type, Color);
            logger.EmptyLine();

            foreach (var modifier in unit.Modifiers)
            {
                if (!menu.ShowHidden && modifier.IsHidden)
                {
                    continue;
                }

                logger.Write("Name: " + modifier.Name, Type, Color);

                if (menu.ShowTextureName)
                {
                    logger.Write("Texture name: " + modifier.TextureName, Type, Color);
                }
                if (menu.ShowElapsedTime)
                {
                    logger.Write("Elapsed time: " + modifier.ElapsedTime, Type, Color);
                }
                if (menu.ShowRemainingTime)
                {
                    logger.Write("Remaining time: " + modifier.RemainingTime, Type, Color);
                }

                logger.EmptyLine();
            }

            logger.EmptyLine();
        }

        private void OnAnimationChanged(Entity sender, EventArgs args)
        {
            var menu = mainMenu.OnChangeMenu.Animations;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Animation;

            logger.Write("Animation changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassID, Type, Color);
            logger.Write("Name: " + sender.Animation.Name, Type, Color);
            logger.EmptyLine();
        }

        private void OnBoolPropertyChanged(Entity sender, BoolPropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var menu = mainMenu.OnChangeMenu.Bools;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Bool;

            logger.Write("Bool property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassID, Type, Color);
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property values: " + args.OldValue + " => " + args.NewValue, Type, Color);
            logger.EmptyLine();
        }

        private void OnEntityAdded(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;

            if (unit != null && unit.IsValid)
            {
                var menu = mainMenu.OnAddRemove.Units;

                if (!menu.OnAddEnabled || menu.IgnoreUseless && Data.IgnoredUnits.Contains(unit.Name))
                {
                    return;
                }

                const Color Color = Color.Green;
                const Logger.Type Type = Logger.Type.Unit;

                logger.Write("Unit added", Type, Color, true);
                logger.Write("Name: " + unit.Name, Type, Color);
                logger.Write("ClassID: " + unit.ClassID, Type, Color);
                logger.Write("Position: " + unit.Position, Type, Color);
                logger.Write("Attack capability: " + unit.AttackCapability, Type, Color);
                logger.Write("Vision: " + unit.DayVision, Type, Color);
                logger.Write("Health: " + unit.Health, Type, Color);
                logger.EmptyLine();

                return;
            }

            var ability = args.Entity as Ability;

            if (ability != null && ability.IsValid)
            {
                var menu = mainMenu.OnAddRemove.Abilities;

                if (!menu.OnAddEnabled || menu.IgnoreUseless && Data.IgnoredAbilities.Any(ability.Name.Contains)
                    || menu.HeroesOnly && !(ability.Owner is Hero))
                {
                    return;
                }

                const Color Color = Color.Green;
                const Logger.Type Type = Logger.Type.Ability;

                logger.Write("Ability added", Type, Color, true);
                logger.Write("Name: " + ability.Name, Type, Color);
                logger.Write("ClassID: " + ability.ClassID, Type, Color);
                logger.Write("Is item: " + (ability is Item), Type, Color);
                logger.Write("Level: " + ability.Level, Type, Color);
                logger.Write("Owner name: " + ability.Owner?.Name, Type, Color);
                logger.Write("Owner classID: " + ability.Owner?.ClassID, Type, Color);
                logger.EmptyLine();
            }
        }

        private void OnEntityRemoved(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;

            if (unit != null && unit.IsValid)
            {
                var menu = mainMenu.OnAddRemove.Units;

                if (!menu.OnRemoveEnabled || menu.IgnoreUseless && Data.IgnoredUnits.Contains(unit.Name))
                {
                    return;
                }

                const Color Color = Color.Red;
                const Logger.Type Type = Logger.Type.Unit;

                logger.Write("Unit removed", Type, Color, true);
                logger.Write("Name: " + unit.Name, Type, Color);
                logger.Write("ClassID: " + unit.ClassID, Type, Color);
                logger.Write("Position: " + unit.Position, Type, Color);
                logger.EmptyLine();

                return;
            }

            var ability = args.Entity as Ability;

            if (ability != null && ability.IsValid)
            {
                var menu = mainMenu.OnAddRemove.Abilities;

                if (!menu.OnRemoveEnabled || menu.IgnoreUseless && Data.IgnoredAbilities.Any(ability.Name.Contains)
                    || menu.HeroesOnly && !(ability.Owner is Hero))
                {
                    return;
                }

                const Color Color = Color.Red;
                const Logger.Type Type = Logger.Type.Ability;

                logger.Write("Ability removed", Type, Color, true);
                logger.Write("Name: " + ability.Name, Type, Color);
                logger.Write("ClassID: " + ability.ClassID, Type, Color);
                logger.Write("Is item: " + (ability is Item), Type, Color);
                logger.Write("Level: " + ability.Level, Type, Color);
                logger.Write("Owner name: " + ability.Owner?.Name, Type, Color);
                logger.Write("Owner classID: " + ability.Owner?.ClassID, Type, Color);
                logger.EmptyLine();
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            switch (args.Order)
            {
                case Order.Ability:
                case Order.AbilityLocation:
                case Order.AbilityTarget:
                case Order.AbilityTargetRune:
                case Order.AbilityTargetTree:
                case Order.ToggleAbility:
                    if (!mainMenu.OnExecuteOrderMenu.Abilities.Enabled)
                    {
                        return;
                    }
                    break;
                case Order.AttackLocation:
                case Order.AttackTarget:
                case Order.MoveLocation:
                case Order.MoveTarget:
                case Order.Stop:
                case Order.Hold:
                case Order.Continue:
                case Order.Patrol:
                    if (!mainMenu.OnExecuteOrderMenu.AttackMove.Enabled)
                    {
                        return;
                    }
                    break;
                default:
                    if (!mainMenu.OnExecuteOrderMenu.Other.Enabled)
                    {
                        return;
                    }
                    break;
            }

            const Color Color = Color.Magenta;
            const Logger.Type Type = Logger.Type.ExecuteOrder;

            logger.Write("Executed order", Type, Color, true);
            logger.Write("Sender name: " + args.Entities.FirstOrDefault()?.Name, Type, Color);
            logger.Write("Sender classID: " + args.Entities.FirstOrDefault()?.ClassID, Type, Color);
            logger.Write("Order: " + args.Order, Type, Color);
            if (args.Ability != null)
            {
                logger.Write("Ability name: " + args.Ability?.Name, Type, Color);
                logger.Write("Ability classID: " + args.Ability?.ClassID, Type, Color);
            }
            if (!args.TargetPosition.IsZero)
            {
                logger.Write("Position: " + args.TargetPosition, Type, Color);
            }
            if (args.Target != null)
            {
                logger.Write("Target name: " + args.Target.Name, Type, Color);
                logger.Write("Target classID: " + args.Target.ClassID, Type, Color);
            }
            logger.EmptyLine();
        }

        private void OnFloatPropertyChanged(Entity sender, FloatPropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var menu = mainMenu.OnChangeMenu.Floats;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero)
                || menu.IgnoreUseless && Data.IgnoredFloats.Contains(args.PropertyName))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Float;

            logger.Write("Float property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassID, Type, Color);
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property values: " + args.OldValue + " => " + args.NewValue, Type, Color);
            logger.EmptyLine();
        }

        private void OnHandlePropertyChanged(Entity sender, HandlePropertyChangeEventArgs args)
        {
            var menu = mainMenu.OnChangeMenu.Handles;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Handle;

            logger.Write("Handle property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassID, Type, Color);
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property value: " + args.OldValue?.Name, Type, Color);
            logger.EmptyLine();
        }

        private void OnInt32PropertyChanged(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var menu = mainMenu.OnChangeMenu.Int32;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero)
                || menu.IgnoreUseless && Data.IgnoredInt32.Contains(args.PropertyName))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Int32;

            logger.Write("Int32 property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassID, Type, Color);
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property values: " + args.OldValue + " => " + args.NewValue, Type, Color);
            logger.EmptyLine();
        }

        private void OnInt64PropertyChanged(Entity sender, Int64PropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var menu = mainMenu.OnChangeMenu.Int64;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.Int64;

            logger.Write("Int64 property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassID, Type, Color);
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property values: " + args.OldValue + " => " + args.NewValue, Type, Color);
            logger.EmptyLine();
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            var menu = mainMenu.OnAddRemove.Modifiers;

            if (!menu.OnAddEnabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            var modifier = args.Modifier;

            DelayAction.Add(
                1,
                () => {
                    if (modifier == null || !modifier.IsValid
                        || menu.IgnoreUseless && Data.IgnoredModifiers.Contains(modifier.Name))
                    {
                        return;
                    }

                    const Color Color = Color.Green;
                    const Logger.Type Type = Logger.Type.Modifier;

                    logger.Write("Modifier added", Type, Color, true);
                    logger.Write("Sender name: " + sender?.Name, Type, Color);
                    logger.Write("Sender classID: " + sender?.ClassID, Type, Color);
                    logger.Write("Name: " + modifier.Name, Type, Color);
                    logger.Write("Texture name: " + modifier.TextureName, Type, Color);
                    logger.Write("Elapsed time: " + modifier.ElapsedTime, Type, Color);
                    logger.Write("Remaining time: " + modifier.RemainingTime, Type, Color);
                    logger.EmptyLine();
                });
        }

        private void OnModifierRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            var menu = mainMenu.OnAddRemove.Modifiers;

            if (!menu.OnRemoveEnabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            var modifier = args.Modifier;

            if (modifier == null || !modifier.IsValid
                || menu.IgnoreUseless && Data.IgnoredModifiers.Contains(modifier.Name))
            {
                return;
            }

            const Color Color = Color.Red;
            const Logger.Type Type = Logger.Type.Modifier;

            logger.Write("Modifier removed", Type, Color, true);
            logger.Write("Sender name: " + sender?.Name, Type, Color);
            logger.Write("Sender classID: " + sender?.ClassID, Type, Color);
            logger.Write("Name: " + modifier.Name, Type, Color);
            logger.Write("Texture name: " + modifier.TextureName, Type, Color);
            logger.EmptyLine();
        }

        private void OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            var menu = mainMenu.OnAddRemove.Particles;

            if (!menu.OnAddEnabled || menu.HeroesOnly && !args.Name.Contains("hero"))
            {
                return;
            }

            var particle = args.ParticleEffect;

            DelayAction.Add(
                1,
                () => {
                    if (particle == null || !particle.IsValid
                        || menu.IgnoreUseless && Data.IgnoredParctiles.Any(args.Name.Contains))
                    {
                        return;
                    }

                    const Color Color = Color.Green;
                    const Logger.Type Type = Logger.Type.Partcile;

                    logger.Write("Particle added", Type, Color, true);
                    logger.Write("Name: " + args.Name, Type, Color);
                    logger.Write("Highest control point: " + particle.HighestControlPoint, Type, Color);

                    if (menu.ShowControlPointValues)
                    {
                        for (var i = 0u; i <= args.ParticleEffect.HighestControlPoint; i++)
                        {
                            var point = args.ParticleEffect.GetControlPoint(i);
                            if (menu.IgnoreZeroControlPointValues && point.IsZero)
                            {
                                continue;
                            }

                            logger.Write("CP: " + i + " => " + point, Type, Color);
                        }
                    }

                    logger.EmptyLine();
                });
        }

        private void OnStringPropertyChanged(Entity sender, StringPropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return;
            }

            var menu = mainMenu.OnChangeMenu.Strings;

            if (!menu.Enabled || menu.HeroesOnly && !(sender is Hero))
            {
                return;
            }

            const Color Color = Color.Cyan;
            const Logger.Type Type = Logger.Type.String;

            logger.Write("String property changed", Type, Color, true);
            logger.Write("Sender name: " + sender.Name, Type, Color);
            logger.Write("Sender classID: " + sender.ClassID, Type, Color);
            logger.Write("Property name: " + args.PropertyName, Type, Color);
            logger.Write("Property values: " + args.OldValue + " => " + args.NewValue, Type, Color);
            logger.EmptyLine();
        }

        private void OnTrackingProjectileAdded(TrackingProjectileEventArgs args)
        {
            var menu = mainMenu.OnAddRemove.Projectiles;

            if (!menu.OnAddEnabled || menu.HeroesOnly && !(args.Projectile?.Source is Hero))
            {
                return;
            }

            var projectile = args.Projectile;

            if (projectile?.Source == null)
            {
                return;
            }

            const Color Color = Color.Green;
            const Logger.Type Type = Logger.Type.Projectile;

            logger.Write("Projectile added", Type, Color, true);
            logger.Write("Source name: " + projectile.Source.Name, Type, Color);
            logger.Write("Source classID: " + projectile.Source.ClassID, Type, Color);
            logger.Write("Speed: " + projectile.Speed, Type, Color);
            logger.Write("Position: " + projectile.Position, Type, Color);
            logger.Write("Target name: " + projectile.Target?.Name, Type, Color);
            logger.Write("Target classID: " + projectile.Target?.ClassID, Type, Color);
            logger.Write("Target position: " + projectile.TargetPosition, Type, Color);
            logger.EmptyLine();
        }

        private void OnTrackingProjectileRemoved(TrackingProjectileEventArgs args)
        {
            var menu = mainMenu.OnAddRemove.Projectiles;

            if (!menu.OnRemoveEnabled || menu.HeroesOnly && !(args.Projectile?.Source is Hero))
            {
                return;
            }

            var projectile = args.Projectile;

            if (projectile?.Source == null)
            {
                return;
            }

            const Color Color = Color.Red;
            const Logger.Type Type = Logger.Type.Projectile;

            logger.Write("Projectile removed", Type, Color, true);
            logger.Write("Source name: " + projectile.Source.Name, Type, Color);
            logger.Write("Source classID: " + projectile.Source.ClassID, Type, Color);
            logger.Write("Speed: " + projectile.Speed, Type, Color);
            logger.Write("Position: " + projectile.Position, Type, Color);
            logger.Write("Target name: " + projectile.Target?.Name, Type, Color);
            logger.Write("Target classID: " + projectile.Target?.ClassID, Type, Color);
            logger.Write("Target position: " + projectile.TargetPosition, Type, Color);
            logger.EmptyLine();
        }

        private void SpellsOnDump(object sender, EventArgs eventArgs)
        {
            var unit = ObjectManager.LocalPlayer.Selection.FirstOrDefault() as Unit;

            if (unit == null || !unit.IsValid)
            {
                return;
            }

            var menu = mainMenu.DumpingMenu.Spells;

            const Color Color = Color.Yellow;
            const Logger.Type Type = Logger.Type.Spell;

            logger.Write("Spell dump", Type, Color, true);
            logger.Write("Unit name: " + unit.Name, Type, Color);
            logger.Write("Unit classID: " + unit.ClassID, Type, Color);
            logger.EmptyLine();

            foreach (var spell in unit.Spellbook.Spells)
            {
                if (!menu.ShowHidden && spell.IsHidden)
                {
                    continue;
                }

                if (!menu.ShowTalents && spell.Name.StartsWith("special_"))
                {
                    continue;
                }

                logger.Write("Name: " + spell.Name, Type, Color);
                logger.Write("ClassID: " + spell.ClassID, Type, Color);

                if (menu.ShowLevels)
                {
                    logger.Write("Level: " + spell.Level, Type, Color);
                }
                if (menu.ShowManaCost)
                {
                    logger.Write("Mana cost: " + spell.ManaCost, Type, Color);
                }
                if (menu.ShowCastRange)
                {
                    logger.Write("Cast range: " + spell.GetCastRange(), Type, Color);
                }
                if (menu.ShowBehavior)
                {
                    logger.Write("Behavior: " + spell.AbilityBehavior, Type, Color);
                }
                if (menu.ShowTargetType)
                {
                    logger.Write("Target type: " + spell.TargetType, Type, Color);
                    logger.Write("Target team type: " + spell.TargetTeamType, Type, Color);
                }
                if (menu.ShowSpecialData)
                {
                    logger.Write("Special data =>", Type, Color);
                    foreach (var abilitySpecialData in spell.AbilitySpecialData)
                    {
                        logger.Write("  " + abilitySpecialData.Name + ": " + abilitySpecialData.Value, Type, Color);
                    }
                }

                logger.EmptyLine();
            }

            logger.EmptyLine();
        }

        private void UnitsOnDump(object sender, EventArgs e)
        {
            var unit = ObjectManager.LocalPlayer.Selection.FirstOrDefault() as Unit;

            if (unit == null || !unit.IsValid)
            {
                return;
            }

            const Color Color = Color.Yellow;
            const Logger.Type Type = Logger.Type.Unit;

            logger.Write("Unit dump", Type, Color, true);
            logger.Write("Name: " + unit.Name, Type, Color);
            logger.Write("ClassID: " + unit.ClassID, Type, Color);
            logger.Write("Level: " + unit.Level, Type, Color);
            logger.Write("Position: " + unit.Position, Type, Color);
            logger.Write("Team: " + unit.Team, Type, Color);
            logger.Write("Health: " + unit.Health + "/" + unit.MaximumHealth, Type, Color);
            logger.Write("Mana: " + (int)unit.Mana + "/" + (int)unit.MaximumMana, Type, Color);
            logger.Write("AttackCapability: " + unit.AttackCapability, Type, Color);
            logger.Write("Day vision: " + unit.DayVision, Type, Color);
            logger.Write("Night vision: " + unit.NightVision, Type, Color);
            logger.Write("State: " + unit.UnitState, Type, Color);
            logger.EmptyLine();
            logger.Write("Spells =>", Type, Color);
            logger.Write(
                "  Talents count: " + unit.Spellbook.Spells.Count(x => x.Name.StartsWith("special_")),
                Type,
                Color);
            logger.Write(
                "  Active spells count: "
                + unit.Spellbook.Spells.Count(
                    x => !x.Name.StartsWith("special_") && x.AbilityBehavior != AbilityBehavior.Passive),
                Type,
                Color);
            logger.Write(
                "  Passive spells count: "
                + unit.Spellbook.Spells.Count(
                    x => !x.Name.StartsWith("special_") && x.AbilityBehavior == AbilityBehavior.Passive),
                Type,
                Color);
            if (unit.HasInventory)
            {
                logger.EmptyLine();
                logger.Write("Items =>", Type, Color);
                logger.Write("  Inventory Items count: " + unit.Inventory.Items.Count(), Type, Color);
                logger.Write("  Backpack Items count: " + unit.Inventory.Backpack.Count(), Type, Color);
                logger.Write("  Stash Items count: " + unit.Inventory.Stash.Count(), Type, Color);
            }
            logger.EmptyLine();
            logger.Write("Modifiers =>", Type, Color);
            logger.Write("  Active modifiers count: " + unit.Modifiers.Count(x => !x.IsHidden), Type, Color);
            logger.Write("  Hidden modifiers count: " + unit.Modifiers.Count(x => x.IsHidden), Type, Color);
            logger.EmptyLine();
        }

        #endregion
    }
}