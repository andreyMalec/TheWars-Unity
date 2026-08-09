public interface PlayerInput {
    public sealed class UpgradeBase : PlayerInput {
    }

    public sealed class SpecialWeapon : PlayerInput {
    }

    public sealed class SpawnUnit : PlayerInput {
        public int UnitIndex { get; }

        public SpawnUnit(int unitIndex) {
            UnitIndex = unitIndex;
        }
    }

    public sealed class BuySlot : PlayerInput {
    }

    public sealed class BuildTurret : PlayerInput {
        public int TurretIndex { get; }
        public int SlotIndex { get; }

        public BuildTurret(int turretIndex, int slotIndex) {
            TurretIndex = turretIndex;
            SlotIndex = slotIndex;
        }
    }

    public sealed class DestroyTurret : PlayerInput {
        public int SlotIndex { get; }

        public DestroyTurret(int slotIndex) {
            SlotIndex = slotIndex;
        }
    }
}