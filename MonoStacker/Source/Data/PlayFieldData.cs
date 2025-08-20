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
    public struct PlayFieldData
    {
        // based off of Puyo Puyo Tetris
        public BoardDisplaySetting displaySetting = BoardDisplaySetting.ShowMeter;
        public ITetrominoFactory factory = new SrsFactory();
        public Point spawnAreaOffset = new Point(3, 18);
        public IRandomizer randomizer = new SevenBagRandomizer();
        public IRotationSystem rotationSystem = new SuperRotationSys();
        public SpinDenotation parsedSpins = SpinDenotation.TSpinOnly;
        public bool temporaryLandingSys = true;
        public float lineClearDelay = 0;
        public float arrivalDelay = 0;
        public float softLockDelay = .5f;
        public int horiStepResets = 15;
        public bool horiStepResetAllowed = true;
        public int rotateResets = 6;
        public bool rotateResetAllowed = true;
        public bool vertStepResetAllowed = false;
        public bool allowDas = true;
        public float autoshiftDelay = .15f;
        public float autoshiftRepeatRate = 1;
        public bool softDropLocks = false;
        public float gravity = .03f;
        public float softDropFactor = 20;
        public int queueLength = 5;
        public QueueType queueType = QueueType.Sides;
        public bool holdEnabled = true;
        public ComboType comboType = ComboType.Conventional;
        public bool singlesBreakCombo = false;

        public PlayFieldData() { }
    }
}
