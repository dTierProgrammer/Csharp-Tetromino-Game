using Microsoft.Xna.Framework;
using MonoStacker.Source.GameObj;
using MonoStacker.Source.GameObj.Tetromino.Factory;
using MonoStacker.Source.GameObj.Tetromino.Randomizer;
using MonoStacker.Source.Generic;
using MonoStacker.Source.Generic.Rotation;
using MonoStacker.Source.Generic.Rotation.RotationSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Data
{
    public static class PlayFieldPresets
    {
        public static PlayFieldData GuidelineSlow1 = new PlayFieldData()
        {
            bufferType = BufferType.Tap,
            displaySetting = BoardDisplaySetting.ShowMeter,
            factory = new SrsFactory(),
            spawnAreaOffset = new Point(3, 18),
            randomizer = new SevenBagRandomizer(),
            rotationSystem = new SuperRotationSys(),
            parsedSpins = SpinDenotation.TSpinSpecific,
            temporaryLandingSys = true,
            lineClearDelay = .5f,
            arrivalDelay = .11667f,
            softLockDelay = .5f,
            horiStepResets = 15,
            horiStepResetAllowed = true,
            rotateResets = 6,
            rotateResetAllowed = true,
            vertStepResetAllowed = false,
            allowDas = true,
            autoshiftDelay = .15f,
            autoshiftRepeatRate = .5f,
            softDropLocks = false,
            gravity = .03f,
            softDropFactor = 20,
            queueLength = 3,
            queueType = QueueType.Sides,
            holdEnabled = true,
            comboType = ComboType.Conventional,
            singlesBreakCombo = false,
            dasCut = 0.0f,
            hardDropCut = 0.0f,
            lineClearDelayType = LineClearDelayType.IndividualDelay
        };

        public static PlayFieldData GuideLineInf = new PlayFieldData()
        {
            bufferType = BufferType.Tap,
            displaySetting = BoardDisplaySetting.ShowMeter,
            factory = new SrsFactory(),
            spawnAreaOffset = new Point(3, 18),
            randomizer = new SevenBagRandomizer(),
            rotationSystem = new SuperRotationSys(),
            parsedSpins = SpinDenotation.TSpinOnly,
            temporaryLandingSys = true,
            lineClearDelay = .5f,
            arrivalDelay = .11667f,
            softLockDelay = .5f,
            horiStepResets = 15,
            horiStepResetAllowed = true,
            rotateResets = 6,
            rotateResetAllowed = true,
            vertStepResetAllowed = true,
            allowDas = true,
            autoshiftDelay = .15f,
            autoshiftRepeatRate = .5f,
            softDropLocks = false,
            gravity = .03f,
            softDropFactor = 20,
            queueLength = 3,
            queueType = QueueType.Sides,
            holdEnabled = true,
            comboType = ComboType.Conventional,
            singlesBreakCombo = false,
            dasCut = 0.0f,
            hardDropCut = 0.0f,
            lineClearDelayType = LineClearDelayType.IndividualDelay
        };
        

        public static PlayFieldData GuidelineSlow2 = new PlayFieldData()
        {
            bufferType = BufferType.Tap,
            displaySetting = BoardDisplaySetting.ShowMeter,
            factory = new SrsFactory(),
            spawnAreaOffset = new Point(3, 18),
            randomizer = new SevenBagRandomizer(),
            rotationSystem = new SuperRotationSys(),
            parsedSpins = SpinDenotation.TSpinOnly,
            temporaryLandingSys = true,
            lineClearDelay = .5f,
            arrivalDelay = .11667f,
            softLockDelay = .5f,
            horiStepResets = 15,
            horiStepResetAllowed = true,
            rotateResets = 6,
            rotateResetAllowed = true,
            vertStepResetAllowed = false,
            allowDas = true,
            autoshiftDelay = .15f,
            autoshiftRepeatRate = 1,
            softDropLocks = false,
            gravity = .03f,
            softDropFactor = 20,
            queueLength = 5,
            queueType = QueueType.Sides,
            holdEnabled = true,
            comboType = ComboType.Conventional,
            singlesBreakCombo = false,
            dasCut = 0.0f,
            hardDropCut = 0.0f
        };

        public static PlayFieldData GuidelineFast1 = new PlayFieldData()
        {
            bufferType = BufferType.Hold,
            displaySetting = BoardDisplaySetting.ShowMeter,
            factory = new SrsFactory(),
            spawnAreaOffset = new Point(3, 18),
            randomizer = new SevenBagRandomizer(),
            rotationSystem = new SuperRotationSys(),
            parsedSpins = SpinDenotation.TSpinOnly,
            temporaryLandingSys = true,
            lineClearDelay = .0f,
            arrivalDelay = .0f,
            softLockDelay = .5f,
            horiStepResets = 15,
            horiStepResetAllowed = true,
            rotateResets = 6,
            rotateResetAllowed = true,
            vertStepResetAllowed = false,
            allowDas = true,
            autoshiftDelay = .15f,
            autoshiftRepeatRate = 1f,
            softDropLocks = false,
            gravity = .03f,
            softDropFactor = 20,
            queueLength = 5,
            queueType = QueueType.Sides,
            holdEnabled = true,
            comboType = ComboType.Conventional,
            singlesBreakCombo = false,
            dasCut = 0.048f,
            hardDropCut = 0.048f,
            softDropType = SoftDropType.Set,
            softDropAmount = 500,
        };

        public static PlayFieldData GuidelineFast2 = new PlayFieldData()
        {
            bufferType = BufferType.Hold,
            displaySetting = BoardDisplaySetting.ShowMeter,
            factory = new SrsFactory(),
            spawnAreaOffset = new Point(3, 18),
            randomizer = new SevenBagRandomizer(),
            rotationSystem = new SuperRotationSys(),
            parsedSpins = SpinDenotation.TSpinOnly,
            temporaryLandingSys = true,
            lineClearDelay = .0f,
            arrivalDelay = .0f,
            softDropType = SoftDropType.Set,
            softDropAmount = 500,
            softLockDelay = 15f,
            horiStepResets = 15,
            horiStepResetAllowed = true,
            rotateResets = 6,
            rotateResetAllowed = true,
            vertStepResetAllowed = false,
            allowDas = true,
            autoshiftDelay = .15f,
            autoshiftRepeatRate = 10f,
            softDropLocks = false,
            gravity = .03f,
            softDropFactor = 20,
            queueLength = 5,
            queueType = QueueType.Sides,
            holdEnabled = true,
            comboType = ComboType.Conventional,
            singlesBreakCombo = false,
            dasCut = 0.048f,
            hardDropCut = 0.048f
        };

        public static PlayFieldData Arcade0 = new PlayFieldData()
        {
            
            bufferType = BufferType.None,
            displaySetting = BoardDisplaySetting.ShowMeter,
            factory = new ArcadeFactory(),
            spawnAreaOffset = new Point(3, 20),
            randomizer = new UnbiasedRandomizer(),
            rotationSystem = new BasicRotationSys(),
            parsedSpins = SpinDenotation.None,
            temporaryLandingSys = false,
            lineClearDelay = .6972f,
            arrivalDelay = .5f,
            softDropType = SoftDropType.Set,
            softDropAmount = 1,
            softLockDelay = .5f,
            horiStepResetAllowed = false,
            rotateResetAllowed = false,
            vertStepResetAllowed = true,
            allowDas = true,
            autoshiftDelay = .332f,
            autoshiftRepeatRate = 1f,
            softDropLocks = true,
            gravity = .03f,
            queueLength = 1,
            queueType = QueueType.Top,
            holdEnabled = false,
            comboType = ComboType.Disable,
            
        };

        public static PlayFieldData Arcade1 = new PlayFieldData()
        {
            bufferType = BufferType.Hold,
            displaySetting = BoardDisplaySetting.ShowMeter,
            factory = new ArcadeFactory(),
            spawnAreaOffset = new Point(3, 20),
            randomizer = new MasterRandomizer(),
            rotationSystem = new ArcadeRotationSys(),
            parsedSpins = SpinDenotation.None,
            temporaryLandingSys = true,
            lineClearDelay = .6806f,
            arrivalDelay = .5f,
            softDropType = SoftDropType.Set,
            softDropAmount = 1,
            softLockDelay = .5f,
            horiStepResetAllowed = false,
            rotateResetAllowed = false,
            vertStepResetAllowed = true,
            allowDas = true,
            autoshiftDelay = .2656f,
            autoshiftRepeatRate = 1f,
            softDropLocks = true,
            gravity = .03f,
            queueLength = 1,
            queueType = QueueType.Top,
            holdEnabled = false,
            comboType = ComboType.Arcade,
            fastDropType = FastDropType.Disable
        };

        public static PlayFieldData Arcade2 = new PlayFieldData()
        {
            bufferType = BufferType.Hold,
            displaySetting = BoardDisplaySetting.ShowMeter,
            factory = new ArcadeFactory(),
            spawnAreaOffset = new Point(3, 20),
            randomizer = new TARandomizer(),
            rotationSystem = new ArcadeRotationSys(),
            parsedSpins = SpinDenotation.None,
            temporaryLandingSys = true,
            lineClearDelay = .6806f,
            arrivalDelay = .5f,
            softDropType = SoftDropType.Set,
            softDropAmount = 1,
            softLockDelay = .5f,
            horiStepResetAllowed = false,
            rotateResetAllowed = false,
            vertStepResetAllowed = true,
            allowDas = true,
            autoshiftDelay = .2656f,
            autoshiftRepeatRate = 1f,
            softDropLocks = true,
            gravity = .03f,
            queueLength = 1,
            queueType = QueueType.Top,
            holdEnabled = false,
            comboType = ComboType.Arcade,
            fastDropType = FastDropType.FirmDrop
        };

        public static PlayFieldData Arcade3 = new PlayFieldData()
        {
            bufferType = BufferType.Hold,
            displaySetting = BoardDisplaySetting.ShowMeter,
            factory = new ArcadeFactory(),
            spawnAreaOffset = new Point(3, 20),
            randomizer = new TIRandomizer(),
            rotationSystem = new ArcadeRotationSys(),
            parsedSpins = SpinDenotation.None,
            temporaryLandingSys = true,
            lineClearDelay = .6806f,
            arrivalDelay = .5f,
            softDropType = SoftDropType.Set,
            softDropAmount = 1,
            softLockDelay = .5f,
            horiStepResetAllowed = false,
            rotateResetAllowed = false,
            vertStepResetAllowed = true,
            allowDas = true,
            autoshiftDelay = .2656f,
            autoshiftRepeatRate = 1f,
            softDropLocks = true,
            gravity = .03f,
            queueLength = 3,
            queueType = QueueType.Top,
            holdEnabled = true,
            comboType = ComboType.Arcade,
            fastDropType = FastDropType.FirmDrop
        };
    }
}
