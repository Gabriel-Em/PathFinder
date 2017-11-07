using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathFinder
{
    public partial class GUI : Form
    {
        private MapState Map;
        private PathFinder PF;
        private List<Block> shortestPath;
        private List<Block> algorithmPath;
        private int pathIndex;
        private int mapState;
        
        // States of the map
        //
        // 0 = constructing the map
        // 1 = during the animation
        // 2 = after the animation

        private int selectedBlockIndex;

        // 0 = Empty
        // 1 = Wall
        // 2 = Player
        // 3 = Destination

        private int animationState;

        // 0 = animating all visited blocks
        // 1 = traceback to origin
        // 2 = animating shortest path

        private Image blockWallImage = Properties.Resources.BrickBlock;
        private Image blockDestinationImage = Properties.Resources.DestinationBlock;
        private Image blockPlayerImage = Properties.Resources.PlayerBlock;
        private Image blockEmptyImage = Properties.Resources.EmptyBlock;
        private Image blockOriginalPlayerImage = Properties.Resources.OriginalPlayerBlock;
        private Image blockPlayerAtDestinationImage = Properties.Resources.PlayerAtDestinationBlock;
        private Image blockEmptyVisitedImage = Properties.Resources.EmptyVisitedBlock;
        private Image blockEmptyBlockTraceBackImage = Properties.Resources.EmptyBlockTraceBack;
        private Image blockDestinationTraceBackImage = Properties.Resources.DestinationBlockTraceBack;
        private Image blockDestinationVisitedImage = Properties.Resources.DestinationBlockVisited;
        private Image blockPlayerVisitedImage = Properties.Resources.PlayerBlockVisited;
        private Image blockPlayerTraceBackImage = Properties.Resources.PlayerTraceBackBlock;

        private Image pathNSImage = Properties.Resources.PathNS;
        private Image pathWEImage = Properties.Resources.PathWE;
        private Image pathNWImage = Properties.Resources.PathNW;
        private Image pathNEImage = Properties.Resources.PathNE;
        private Image pathSWImage = Properties.Resources.PathSW;
        private Image pathSEImage = Properties.Resources.PathSE;

        public GUI()
        {
            InitializeComponent();

            Map = new MapState();
            PF = new PathFinder();
            selectedBlockIndex = 1;
        }

        // Constructing the map

        private void picBlockType_Click(object sender, EventArgs e)
        {
            switch ((sender as PictureBox).Name)
            {
                case "picBlockEmpty": displaySelectedBlockType(0); selectedBlockIndex = 0; break;
                case "picBlockWall": displaySelectedBlockType(1); selectedBlockIndex = 1; break;
                case "picBlockPlayer": displaySelectedBlockType(2); selectedBlockIndex = 2; break;
                case "picBlockDestination": displaySelectedBlockType(3); selectedBlockIndex = 3; break;  
            }
        }

        private void displaySelectedBlockType(int index)
        {
            switch (index)
            {
                case 0: picBlockSelector.Location = new Point(picBlockEmpty.Location.X - 3, picBlockEmpty.Location.Y - 3); break;
                case 1: picBlockSelector.Location = new Point(picBlockWall.Location.X - 3, picBlockWall.Location.Y - 3); break;
                case 2: picBlockSelector.Location = new Point(picBlockPlayer.Location.X - 3, picBlockPlayer.Location.Y - 3); break;
                case 3: picBlockSelector.Location = new Point(picBlockDestination.Location.X - 3, picBlockDestination.Location.Y - 3); break;

            }
        }

        private void picMapTile_Click(object sender, EventArgs e)
        {
            if (mapState == 0)  // if there's no animation going on
            {
                PictureBox pb = (sender as PictureBox);
                string[] coord = pb.Name.Substring(10).Split('x');

                switch (selectedBlockIndex)
                {
                    case 0: pb.BackgroundImage = blockEmptyImage; break;
                    case 1: pb.BackgroundImage = blockWallImage; break;
                    case 2: clearPlayerBlock(); pb.BackgroundImage = blockPlayerImage; break;
                    case 3: clearDestinationBlock(); pb.BackgroundImage = blockDestinationImage; break;
                }

                Map.setBlockType(Int32.Parse(coord[0]) - 1, Int32.Parse(coord[1]) - 1, selectedBlockIndex);
            }
        }

        private void clearPlayerBlock()
        {
            Tuple<int, int> playerCoord = Map.getPlayerBlockCoord();

            if (playerCoord != null)
                setBlockImage(playerCoord, 0);
        }

        private void clearDestinationBlock()
        {
            Tuple<int, int> destCoord = Map.getDestinationBlockCoord();

            if (destCoord != null)
                setBlockImage(destCoord, 0);
        }

        // Menu

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Map.getPlayerBlockCoord() != null && Map.getDestinationBlockCoord() != null)
            {
                Tuple<List<Block>, List<Block>> Result = PF.A_star(new MapState(Map));

                if (Result != null)
                {
                    shortestPath = Result.Item1;
                    algorithmPath = Result.Item2;

                    mapState = 1;
                    animationState = 0;

                    btnStart.Enabled = false;
                    btnRestart.Enabled = false;
                    btnReset.Enabled = false;

                    pathIndex = -1;
                    timerPathAnimation.Start();
                }
                else
                    MessageBox.Show("The Player cannot reach its destination.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("You must place the \"Player\" and the \"Destination\" first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            Restart();
            mapState = 0;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Map.Reset();
            Restart();
            mapState = 0;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Other Methods and Events

        private void Restart()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 19; j++)
                    setBlockImage(new Tuple<int, int>(i, j), Map.getBlock(i, j).Type);

            btnStart.Enabled = true;
        }

        private void setBlockImage(Tuple<int, int> Coord, int imageType)
        {
            switch (imageType)
            {
                case 0: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockEmptyImage; break;
                case 1: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockWallImage; break;
                case 2: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockPlayerImage; break;
                case 3: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockDestinationImage; break;
                case 4: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockOriginalPlayerImage; break;
                case 5: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockPlayerAtDestinationImage; break;
                case 6: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = pathNSImage; break;
                case 7: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = pathWEImage; break;
                case 8: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = pathNWImage; break;
                case 9: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = pathNEImage; break;
                case 10: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = pathSWImage; break;
                case 11: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = pathSEImage; break;
                case 12: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockEmptyVisitedImage; break;
                case 13: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockEmptyBlockTraceBackImage; break;
                case 14: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockDestinationTraceBackImage; break;
                case 15: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockDestinationVisitedImage; break;
                case 16: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockPlayerVisitedImage; break;
                case 23: (panelMap.Controls.Find("picMapTile" + (Coord.Item1 + 1) + "x" + (Coord.Item2 + 1), false)[0] as PictureBox).BackgroundImage = blockPlayerTraceBackImage; break;
            }
        }

        private void timerPathAnimation_Tick(object sender, EventArgs e)
        {
            if (animationState == 0)    // marking all visited blocks
            {

                if (pathIndex < algorithmPath.Count - 1)
                {
                    pathIndex++;

                  //  MessageBox.Show("G = " + algorithmPath[pathIndex].G + ", H = " + +algorithmPath[pathIndex].H + ", F = " + +algorithmPath[pathIndex].F);
                    if (pathIndex == 0)
                        setBlockImage(new Tuple<int, int>(algorithmPath[pathIndex].Coord.Item1, algorithmPath[pathIndex].Coord.Item2), 16);
                    else
                    {
                        if (pathIndex < algorithmPath.Count - 1)
                            setBlockImage(new Tuple<int, int>(algorithmPath[pathIndex].Coord.Item1, algorithmPath[pathIndex].Coord.Item2), 12);
                        else
                            setBlockImage(new Tuple<int, int>(algorithmPath[pathIndex].Coord.Item1, algorithmPath[pathIndex].Coord.Item2), 15);
                    }
                }
                else
                {
                    pathIndex = shortestPath.Count;
                    animationState = 1;
                }
            }
            else
            if (animationState == 1)    // traceback
            {
                if (pathIndex > 0)
                {
                    pathIndex--;
                    if (pathIndex == shortestPath.Count - 1)
                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex].Coord.Item1, shortestPath[pathIndex].Coord.Item2), 14);
                    else
                    {
                        if (pathIndex > 0)
                            setBlockImage(new Tuple<int, int>(shortestPath[pathIndex].Coord.Item1, shortestPath[pathIndex].Coord.Item2), 13);
                        else
                            setBlockImage(new Tuple<int, int>(shortestPath[pathIndex].Coord.Item1, shortestPath[pathIndex].Coord.Item2), 23);
                    }
                }
                else
                {
                    pathIndex = 0;
                    animationState = 2;
                }
            }
            else    // travelling to destination
            {
                if (pathIndex < shortestPath.Count - 1)
                {
                    pathIndex++;

                    if (pathIndex - 2 < 0) // first step we take has a special animation
                    {
                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex].Coord.Item1, shortestPath[pathIndex].Coord.Item2), 16);
                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 4);
                    }
                    else
                    {
                        if (pathIndex == shortestPath.Count - 1) // last step we take has a special animation
                            setBlockImage(new Tuple<int, int>(shortestPath[pathIndex].Coord.Item1, shortestPath[pathIndex].Coord.Item2), 5);
                        else
                            setBlockImage(new Tuple<int, int>(shortestPath[pathIndex].Coord.Item1, shortestPath[pathIndex].Coord.Item2), 16);

                        string path = String.Empty;

                        if (shortestPath[pathIndex].Coord.Item1 > shortestPath[pathIndex - 2].Coord.Item1)
                            path += "S";
                        else
                            if (shortestPath[pathIndex].Coord.Item1 < shortestPath[pathIndex - 2].Coord.Item1)
                            path += "N";

                        if (shortestPath[pathIndex].Coord.Item2 > shortestPath[pathIndex - 2].Coord.Item2)
                            path += "E";
                        else
                            if (shortestPath[pathIndex].Coord.Item2 < shortestPath[pathIndex - 2].Coord.Item2)
                            path += "W";

                        if (path.Length == 1)
                            switch (path)
                            {
                                case "N": setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 6); break;
                                case "S": setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 6); break;
                                case "W": setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 7); break;
                                case "E": setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 7); break;
                            }
                        else
                            switch (path)
                            {
                                case "NW":
                                    if (shortestPath[pathIndex].Coord.Item1 != shortestPath[pathIndex - 1].Coord.Item1)
                                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 9);
                                    else
                                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 10);
                                    break;
                                case "NE":
                                    if (shortestPath[pathIndex].Coord.Item1 != shortestPath[pathIndex - 1].Coord.Item1)
                                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 8);
                                    else
                                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 11);
                                    break;
                                case "SW":
                                    if (shortestPath[pathIndex].Coord.Item1 != shortestPath[pathIndex - 1].Coord.Item1)
                                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 11);
                                    else
                                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 8);
                                    break;
                                case "SE":
                                    if (shortestPath[pathIndex].Coord.Item1 != shortestPath[pathIndex - 1].Coord.Item1)
                                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 10);
                                    else
                                        setBlockImage(new Tuple<int, int>(shortestPath[pathIndex - 1].Coord.Item1, shortestPath[pathIndex - 1].Coord.Item2), 9);
                                    break;
                            }
                    }
                }
                else
                {
                    mapState = 2;
                    timerPathAnimation.Stop();
                    btnRestart.Enabled = true;
                    btnReset.Enabled = true;
                }
            }
        }
    }
}
