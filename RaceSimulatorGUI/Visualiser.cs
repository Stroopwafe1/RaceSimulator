using Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows.Ink;
using System.Windows.Media.Imaging;

namespace RaceSimulatorGUI {
    public static class Visualiser {

        private const int compass = 1;
        private static List<GridSquare> _gridSquares;
        private static Track _trackCache;
        private static Bitmap _currTrackBitmap;

        #region imagePaths
        private static readonly string _start = ".\\Resources\\Racetrack_Start.png";
        private static readonly string _finish = ".\\Resources\\Racetrack_Finish.png";
        private static readonly string _straight = ".\\Resources\\Racetrack_Straight.png";
        private static readonly string _corner = ".\\Resources\\Racetrack_Corner.png";
        private static readonly string _carBlue = ".\\Resources\\Racetrack_Car_Blue.png";
        private static readonly string _carBlueBroken = ".\\Resources\\Racetrack_Car_Blue_BrokenDown.png";
        private static readonly string _carRed = ".\\Resources\\Racetrack_Car_Red.png";
        private static readonly string _carRedBroken = ".\\Resources\\Racetrack_Car_Red_BrokenDown.png";
        private static readonly string _carGreen = ".\\Resources\\Racetrack_Car_Green.png";
        private static readonly string _carGreenBroken = ".\\Resources\\Racetrack_Car_Green_BrokenDown.png";
        private static readonly string _carGrey = ".\\Resources\\Racetrack_Car_Grey.png";
        private static readonly string _carGreyBroken = ".\\Resources\\Racetrack_Car_Grey_BrokenDown.png";
        private static readonly string _carYellow = ".\\Resources\\Racetrack_Car_Yellow.png";
        private static readonly string _carYellowBroken = ".\\Resources\\Racetrack_Car_Yellow_BrokenDown.png";
        #endregion

        public static void Initialise() {
            _gridSquares = new List<GridSquare>();
            ImageLoader.Initialise();
        }

        /// <summary>
        /// This is just for testing purposes
        /// </summary>
        /// <returns></returns>
        public static List<GridSquare> GetGridSquares() {
            return _gridSquares;
        }

        public static BitmapSource DrawTrack(Track track) {
            Bitmap trackBitmap = DrawBaseTrack(track);
            Bitmap driverBitmap = DrawParticipants(trackBitmap);
            return ImageLoader.CreateBitmapSourceFromGdiBitmap(driverBitmap);
        }

        private static GridSquare GetGridSquare(int x, int y) {
            GridSquare square = _gridSquares.Find(_square => _square.X == x && _square.Y == y);
            return square;
        }


        private static Bitmap GetParticipantImage(IParticipant participant, int compass) {
            Bitmap car = null;
            switch (participant.TeamColour) {
                case TeamColours.Blue:
                    if (participant.Equipment.IsBroken)
                        car = ImageLoader.GetImageFromCache(_carBlueBroken);
                    else
                        car = ImageLoader.GetImageFromCache(_carBlue);
                    break;
                case TeamColours.Green:
                    if (participant.Equipment.IsBroken)
                        car = ImageLoader.GetImageFromCache(_carGreenBroken);
                    else
                        car = ImageLoader.GetImageFromCache(_carGreen);
                    break;
                case TeamColours.Grey:
                    if (participant.Equipment.IsBroken)
                        car = ImageLoader.GetImageFromCache(_carGreyBroken);
                    else
                        car = ImageLoader.GetImageFromCache(_carGrey);
                    break;
                case TeamColours.Red:
                    if (participant.Equipment.IsBroken)
                        car = ImageLoader.GetImageFromCache(_carRedBroken);
                    else
                        car = ImageLoader.GetImageFromCache(_carRed);
                    break;
                case TeamColours.Yellow:
                    if (participant.Equipment.IsBroken)
                        car = ImageLoader.GetImageFromCache(_carYellowBroken);
                    else
                        car = ImageLoader.GetImageFromCache(_carYellow);
                    break;
            }
            Bitmap carCorrectOrientation = new Bitmap(car);
            switch (compass) {
                case 1:
                    carCorrectOrientation.RotateFlip(RotateFlipType.Rotate90FlipNone);
                  break;
                case 2:
                    carCorrectOrientation.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 3:
                    carCorrectOrientation.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            return carCorrectOrientation;
        }
        private static Bitmap DrawParticipants(Bitmap trackBitmap) {
            int maxX = _gridSquares.Max(_square => _square.X);
            int maxY = _gridSquares.Max(_square => _square.Y);
            Bitmap driverBitmap = new Bitmap(trackBitmap);
            Graphics graphics = Graphics.FromImage(driverBitmap);
            for (int y = 0; y <= maxY; y++) {
                for (int x = 0; x <= maxX; x++) {
                    GridSquare square = GetGridSquare(x, y);
                    if (square?.SectionData.Left == null && square?.SectionData.Right == null) continue;
                    Bitmap car = null;
                    if (square.SectionData.Left != null) {
                        IParticipant participant = square.SectionData.Left;
                        car = GetParticipantImage(participant, square.Compass);
                        graphics.DrawImage(car, x * 239 + GetXOffset(true, square.Compass, square.SectionData.DistanceLeft), y * 239 + GetYOffset(true, square.Compass, square.SectionData.DistanceLeft));
                    } 
                    if (square.SectionData.Right != null) { 
                        IParticipant participant = square.SectionData.Right;
                        car = GetParticipantImage(participant, square.Compass);
                        graphics.DrawImage(car, x * 239 + GetXOffset(false, square.Compass, square.SectionData.DistanceRight), y * 239 + GetYOffset(false, square.Compass, square.SectionData.DistanceRight));
                    }
                    
                }
            }
            return driverBitmap;
        }

        private static void MoveParticipantsAtStart() {
            Race race = Data.CurrentRace;
            foreach (var section in race.Track.Sections.Where(section => section.SectionType == SectionTypes.StartGrid)) {
                SectionData data = race.GetSectionData(section);
                switch(compass) {
                    case 0:
                        data.DistanceLeft = 60;
                        data.DistanceRight = 170;
                        break;
                    case 1:
                        data.DistanceLeft = 140;
                        data.DistanceRight = 30;
                        break;
                    case 2:
                        data.DistanceLeft = 140;
                        data.DistanceRight = 15;
                        break;
                    case 3:
                        data.DistanceLeft = 55;
                        data.DistanceRight = 170;
                        break;
                }
            }
        }

        private static int GetXOffset(bool left, int comp, float dist) {
            float distance = dist;
            if (dist > 239) 
                distance = 239f;
            if(left) {
                switch(comp) {
                    case 0:
                        return 68;
                    case 1:
                        return (int)distance;
                    case 2:
                        return 140;
                    case 3:
                        return (int)Math.Abs(distance - 239);
                }
            } else {
                switch (comp) {
                    case 0:
                        return 145;
                    case 1:
                        return (int)distance;
                    case 2:
                        return 68;
                    case 3:
                        return (int)Math.Abs(distance - 239);
                }
            }
            return 0;
        }

        private static int GetYOffset(bool left, int comp, float dist) {
            float distance = dist;
            if (dist > 239)
                distance = 239f;
            if (left) {
                switch (comp) {
                    case 0:
                        return (int)Math.Abs(distance - 239);
                    case 1:
                        return 70;
                    case 2:
                        return (int)distance;
                    case 3:
                        return 140;
                }
            } else {
                switch (comp) {
                    case 0:
                        return (int)Math.Abs(distance - 239);
                    case 1:
                        return 140;
                    case 2:
                        return (int)distance;
                    case 3:
                        return 60;
                }
            }
            return 0;
        }

        private static Bitmap DrawBaseTrack(Track track) {
            if (_trackCache == track) return _currTrackBitmap;
            _trackCache = track;
            CalculateGrid(track.Sections);
            MoveGrid(Math.Abs(GridSquare.LowestX), Math.Abs(GridSquare.LowestY));
            _gridSquares = _gridSquares.OrderBy(_square => _square.Y).ToList();
            int maxX = _gridSquares.Max(_square => _square.X);
            int maxY = _gridSquares.Max(_square => _square.Y);
            Bitmap background = ImageLoader.CreateEmptyBitmap(239 * maxX + 239, 239 * maxY + 239);
            Bitmap empty = ImageLoader.CreateEmptyBitmap(239, 239);
            Graphics graphics = Graphics.FromImage(background);
            for (int y = 0; y <= maxY; y++) {
                for (int x = 0; x <= maxX; x++) {
                    GridSquare square = GetGridSquare(x, y);
                    Bitmap currTile = null;
                    if (square == null) {
                        currTile = empty;
                    } else {
                        currTile = new Bitmap(ImageLoader.GetImageFromCache(square.ImagePath));
                        switch (square.Compass) {
                            case 0:
                                if (square.Flip)
                                    currTile.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                break;
                            case 1:
                                if (square.Flip)
                                    currTile.RotateFlip(RotateFlipType.Rotate90FlipY);
                                else
                                    currTile.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                break;
                            case 2:
                                if (square.Flip)
                                    currTile.RotateFlip(RotateFlipType.Rotate180FlipX);
                                else
                                    currTile.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                break;
                            case 3:
                                if (square.Flip)
                                    currTile.RotateFlip(RotateFlipType.Rotate270FlipY);
                                else
                                    currTile.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                break;
                        }
                    }
                    graphics.DrawImage(currTile, x * 239, y * 239);
                }

            }
            _currTrackBitmap = background;
            return background;
        }

        private static void CalculateGrid(LinkedList<Section> sections) {
            Race race = Data.CurrentRace;
            int comp = compass;
            int x = 0, y = 0;
            _gridSquares?.Clear();
            GridSquare.LowestX = 0;
            GridSquare.LowestY = 0;
            foreach (Section section in sections) {
                SectionTypes type = section.SectionType;
                SectionData data = race.GetSectionData(section);
                switch (type) {
                    case SectionTypes.StartGrid:
                        _gridSquares.Add(new GridSquare(x, y, _start, data, comp));
                        break;
                    case SectionTypes.Straight:
                        _gridSquares.Add(new GridSquare(x, y, _straight, data, comp));
                        break;
                    case SectionTypes.LeftCorner:
                        _gridSquares.Add(new GridSquare(x, y, _corner, data, comp, true));
                        comp = (comp - 1) % 4;
                        if (comp < 0)
                            comp = 3;
                        break;
                    case SectionTypes.RightCorner:
                        _gridSquares.Add(new GridSquare(x, y, _corner, data, comp));
                        comp = (comp + 1) % 4;
                        break;
                    case SectionTypes.Finish:
                        _gridSquares.Add(new GridSquare(x, y, _finish, data, comp));
                        break;
                }
                if (comp == 0) {
                    y--;
                } else if (comp == 1) {
                    x++;
                } else if (comp == 2) {
                    y++;
                } else {
                    x--;
                }
            }
        }

        private static void MoveGrid(int x, int y) {
            foreach (GridSquare square in _gridSquares) {
                square.X += x;
                square.Y += y;
            }
        }
    }
}
