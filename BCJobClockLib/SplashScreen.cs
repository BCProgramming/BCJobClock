using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BASeCamp.Configuration;
using BCJobClockLib;
using Timer=System.Threading.Timer;

namespace BASeBlock
{
    public partial class SplashScreen : Form, iManagerCallback
    {
        private struct drawimagedata
        {
            private Image _DrawImage;
            public Image DrawImage
            {
                get { return _DrawImage; }
                set
                {
                    _DrawImage = value;
                    DrawSize = _DrawImage.Size;
                }
            }
           public PointF Location;
            public double Angle;
            public Size DrawSize;
            public void Draw(Graphics g)
            {
                //Location corresponds to the center of this image...
                var copyt = g.Transform;

                g.TranslateTransform(Location.X, Location.Y);
                double useangle = (float)(Angle * (180d / Math.PI));
                Debug.Print("useangle:" + useangle);
                g.RotateTransform((float)(useangle));

                g.TranslateTransform((float)-DrawImage.Width / 2, (float)-DrawImage.Height / 2);

                //draw the image at 0,0...
                g.DrawImage(DrawImage, 0, 0,DrawSize.Width,DrawSize.Height);
                //reset the transform.
                g.Transform = copyt;
            }
        }


        private System.Threading.Timer delaytimer;
        //private System.Threading.Timer alphatimer;
        private Thread bobberthread;
        // private BobbingTextAnimator Bobber;
        // private BobbingTextAnimator Bobber2;
        private iManagerCallback filelogwriter;
        private Animatable Bobber;
        private Animatable Bobber2;
        private Graphics ImageBackBufferCanvas;
        private Bitmap ImageBackBufferbitmap;
        private Bitmap BackgroundBitmap;
        private Graphics BackgroundBuffer;
        private Thread quickthread;
        private Image useBackground;
        private double mAlphaIncrement = 0.01;
        private bool mProceed = false;
        private bool ShownAsAbout = false;
        private String logfilename;
        private List<drawimagedata> imagesdraw = new List<drawimagedata>();
        private DateTime? BobberThreadEffectFinish = null;
        private DateTime BobberThreadEffectStart;
        private bool performingEffect = false;
        private int EffectCount = 0;
        private PointF prevparticlepos;
        private Random rg = new Random();
        private void DoInit()
        {
            try
            {
                logfilename = Path.Combine(BCBlockGameState.AppDataFolder, "baseblock.log");


                filelogwriter = new FileLogCallback(logfilename);
            }
            catch
            {
            }
        }

        public SplashScreen(bool ShowasAbout)
        {
            ShownAsAbout = ShowasAbout;

            mProceed = true;
            InitializeComponent();
            DoInit();
        }

        public SplashScreen()
        {
            InitializeComponent();
            DoInit();
        }


      bool mmimagesloaded=false;
      Image[] mmimages=null;
      int mmimageframe = 0;
      DateTime lastmmFrameChange = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
        TimeSpan mmFrameDelayTime=new TimeSpan(0,0,0,0,500);
        private void BobberThread()
        {
            int EffectLengthi = 5000;
            if (BobberThreadEffectFinish == null)
            {
                BobberThreadEffectFinish = DateTime.Now;
            }
            const int numeffects = 6;
            while (true)
            {

                lock (ImageBackBufferCanvas)
                {
                    Thread.Sleep(50);
                    Thread.SpinWait(15);
                    while (Form.ActiveForm == null) Thread.Sleep(0);
                    if (!this.Visible)
                    {
                        Thread.Sleep(0); //nothing to do, allow other threads to run.
                    }
                    else
                    {
                        DrawFrame();
                    }
                    if (performingEffect)
                    {
                        bool doflip = false;
                        TimeSpan EffectLength = DateTime.Now - BobberThreadEffectStart;
                        //currently the only supported "effect" ,draws a bunch of particles across the screen halfway. (Y).


                        imagesdraw.Clear();
                        if (EffectLength.TotalMilliseconds > EffectLengthi)
                        {
                            performingEffect = false;
                            prevparticlepos = new PointF(0, 0);
                            BobberThreadEffectFinish = DateTime.Now;
                        }
                        else
                        {
                            double currpercent = (double) EffectLength.TotalMilliseconds/(double) EffectLengthi;
                            int Xpos =
                                (int)
                                (((float) (ImageBackBufferbitmap.Width*1.5)*currpercent) -
                                 ((float) (ImageBackBufferbitmap.Width*0.2f)));

                            Debug.Print("Effect currpercent=" + currpercent);
                            //                        PointF addparticlepos = new PointF(Xpos, ImageBackBufferbitmap.Height / 2);
                            int effectnumber = EffectCount%numeffects;

                            if (effectnumber%2 == 0)
                            {
                                Xpos = ImageBackBufferbitmap.Width - Xpos;
                                doflip = true;
                            }
                            PointF addparticlepos = new PointF(Xpos, 163);
                            if (prevparticlepos.X == 0 && prevparticlepos.Y == 0) prevparticlepos = addparticlepos;
                            float usehue = (((float) currpercent*3)%1)*240;
                            Color particlecolor = new HSLColor(usehue, 240, 120);
                            const float maxoutspeed = 1f;
                            const int numparticles = 25;
                            Image usepic = null;
                            System.Drawing.Size? usesize;

                            switch (effectnumber)
                            {
                                case 0:
                                case 2:
                                    usepic = rdash.BackgroundImage;
                                    if (effectnumber == 2)
                                        addparticlepos = new PointF(addparticlepos.X,
                                                                    ((float) Math.Sin(addparticlepos.X/16)*8) +
                                                                    addparticlepos.Y);


                                    /*for (int addp = 0; addp < numparticles*2; addp++)
                                    {
                                        Particle addparticle = new DustParticle(addparticlepos, (float) (rg.NextDouble()*4),
                                                                                7500, particlecolor);


                                        lock (DrawParticles)
                                        {
                                            DrawParticles.Add(addparticle);
                                        }
                                    }*/

                                    //create a spectrum...
                                    const float numlines = 25;
                                    const int totalsize = 100;
                                    for (int i = 0; i < numlines; i++)
                                    {

                                        float useYoffset = (totalsize/2) + (((float) i/numlines)*totalsize);
                                        float pct = ((float) i/numlines);
                                        float percenthue = (pct*240);
                                        PointF offsetamount = new PointF(addparticlepos.X - prevparticlepos.X,
                                                                         addparticlepos.Y - prevparticlepos.Y);
                                        //PointF firstpoint = new PointF(addparticlepos.X, useYoffset);
                                        //PointF secondpoint = new PointF(firstpoint.X+offsetamount.X,firstpoint.Y+offsetamount.Y);
                                        PointF firstpoint = addparticlepos;
                                        PointF secondpoint = prevparticlepos;
                                        if (firstpoint.X > secondpoint.X)
                                        {
                                            firstpoint = new PointF(firstpoint.X + 2, firstpoint.Y);
                                            secondpoint = new PointF(secondpoint.X - 2, secondpoint.Y);


                                        }
                                        else
                                        {
                                            firstpoint = new PointF(firstpoint.X - 2, firstpoint.Y);
                                            secondpoint = new PointF(secondpoint.X + 2, secondpoint.Y);

                                        }
                                        //float phue = (percenthue + (Xpos/2)%240 )%240;
                                        float phue = percenthue%240;
                                        Color linecoloruse = new HSLColor(phue, 240, 120);
                                        float usevel = (pct - 0.5f)*5;
                                        DustParticle dust1 = new DustParticle(firstpoint, new PointF(0, usevel));
                                        DustParticle dust2 = new DustParticle(secondpoint, new PointF(0, usevel));
                                        LineParticle lp = new LineParticle(dust1, dust2, linecoloruse);



                                        lock (DrawParticles)
                                        {
                                            DrawParticles.Add(lp);


                                            if (Math.Truncate((double) Xpos)%15 == 0)
                                            {
                                                //add a 5-pointed star...
                                                //StarParticle staradd = new StarParticle(addparticlepos, BCBlockGameState.GetRandomVelocity(0,3), 5, 3, 6, Color.Yellow, Color.Black, (float)(rg.NextDouble() * 2));
                                                CharacterDebris staradd = new CharacterDebris(addparticlepos,
                                                                                              BCBlockGameState.
                                                                                                  GetRandomVelocity(0, 3),
                                                                                              Color.Yellow, 8, 18);
                                                DrawParticles.Add(staradd);

                                            }

                                        }






                                    }





                                    /*
                                    Particle addParticleA = new DustParticle(addparticlepos, new PointF(0, -2));
                                    Particle addParticleB = new DustParticle(addparticlepos, new PointF(0, 2));
                                    LineParticle lp = new LineParticle(addParticleA, addParticleB, particlecolor);
                                
                                    lock (DrawParticles)
                                    {
                                        DrawParticles.Add(lp);
                                    }


                                    */
                                    break;
                                case 1:
                                case 3:
                                    usepic = rdash.BackgroundImage;
                                    if (effectnumber == 3)
                                        addparticlepos = new PointF(addparticlepos.X,
                                                                    ((float) Math.Sin(addparticlepos.X/32)*16) +
                                                                    addparticlepos.Y);
                                    for (int addp = 0; addp < numparticles; addp++)
                                    {
                                        float ppercent = (float) addp/(float) numparticles;
                                        float usespeed = (float) (((maxoutspeed*2)*ppercent) - maxoutspeed);
                                        PointF speeduse = new PointF((float) (rg.NextDouble() - 0.5d), usespeed);
                                        particlecolor = new HSLColor(ppercent*240, 240, 120);
                                        //Particle addparticle = new PolyDebris(addparticlepos, rg.NextDouble() * 2, particlecolor, 3, 4, 3, 8);
                                        Particle addparticle = new PolyDebris(addparticlepos, speeduse, particlecolor, 3,
                                                                              4,
                                                                              3, 8);

                                        lock (DrawParticles)
                                        {
                                            DrawParticles.Add(addparticle);
                                        }
                                    }
                                    break;
                                case 4:
                                    usepic = ttoaster.BackgroundImage;
                                    for (int addp = 0; addp < numparticles*2; addp++)
                                    {
                                        particlecolor = Color.FromArgb(rg.Next(255), rg.Next(255), 0);
                                        Particle addparticle = new DustParticle(addparticlepos,
                                                                                (float) (rg.NextDouble()*4),
                                                                                7500, particlecolor);


                                        lock (DrawParticles)
                                        {
                                            DrawParticles.Add(addparticle);
                                        }
                                    }
                                    break;

                                case 5:
                                    /*
                                
                                     */
                                    //megaman sprites...
                                    //since these are loaded

                                    if (!mmimagesloaded)
                                    {

                                        mmimagesloaded = true;
                                        try
                                        {
                                            mmimages = BCBlockGameState.Imageman.getImageFrames("megaman");
                                        }
                                        catch
                                        {
                                            mmimagesloaded = false;

                                        }

                                    }
                                    if (mmimagesloaded)
                                    {
                                        if ((DateTime.Now - lastmmFrameChange) > mmFrameDelayTime)
                                        {
                                            //advance the frame.
                                            mmimageframe++;


                                        }
                                        mmimageframe = mmimageframe%mmimages.Length;
                                        //they are... or should  be, loaded now.


                                        usepic = mmimages[mmimageframe];
                                        usesize = new Size(usepic.Size.Width*3, usepic.Size.Height*3);


                                    }
                                    break;



                                    /*
                            case 3:
    
                                addparticlepos = new PointF(addparticlepos.X, ((float)Math.Sin(addparticlepos.X / 16) * 8) + addparticlepos.Y);
                                for (int addp = 0; addp < numparticles; addp++)
                                {
                                    Particle addparticle = new PolyDebris(addparticlepos, rg.NextDouble()*2, particlecolor, 3, 4, 3, 8);


                                    lock (DrawParticles)
                                    {
                                        DrawParticles.Add(addparticle);
                                    }
                                }
                                break;
                                */
                            }
                            drawimagedata dd = new drawimagedata();
                            dd.DrawImage = (Image) usepic.Clone();

                            if (doflip) dd.DrawImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            dd.Location = addparticlepos;

                            //imagesdraw.Add(new drawimagedata(BCBlockGameState.Imageman.GetLoadedImage("rdash")

                            double ang = BCBlockGameState.GetAngle(prevparticlepos, addparticlepos);
                            Debug.Print("chosen angle=" + ang);
                            dd.Angle = ang;
                            if (doflip) dd.Angle += (float) Math.PI;
                            imagesdraw.Add(dd);
                            prevparticlepos = addparticlepos;
                        }
                    }
                    else if ((DateTime.Now - BobberThreadEffectFinish.Value).TotalSeconds >= 60)
                    {
                        EffectCount++;
                        EffectCount = EffectCount%numeffects;
                        switch (EffectCount)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                EffectLengthi = 5000;
                                if (BCBlockGameState.Soundman != null) //can be null, if you press shift during startup and take a while on the screen presented...
                                {
                                    if (BCBlockGameState.Soundman.HasSound("asboom"))
                                    {
                                        BCBlockGameState.Soundman.PlaySound("asboom", 3.0f);
                                    }
                                }
                                break;
                            case 4:
                                iSoundSourceObject grabsound;
                                if (BCBlockGameState.Soundman != null)
                                {
                                    if (((grabsound = BCBlockGameState.Soundman.GetSoundRnd("TTOASTER")) != null))
                                    {
                                        grabsound.Play(false, 3.0f);
                                    }
                                }
                                break;
                            case 5:
                                if(BCBlockGameState.Soundman!=null) BCBlockGameState.Soundman.PlaySound("ray");
                                break;
                        }
                        Debug.Print("invoking effect #" + EffectCount);
                        performingEffect = true;

                        BobberThreadEffectStart = DateTime.Now;
                    }
                }
            }
        }

        private List<Particle> DrawParticles = new List<Particle>();

        private void DrawFrame()
        {
            try
            {
                Bobber.PerformFrame();
                Bobber2.PerformFrame();
                //panImage.Update();
                Graphics g = ImageBackBufferCanvas;
                lock (g)
                {
                    g.Clear(Color.Transparent);
                    g.ResetTransform();
                    //g.DrawImage(panImage.BackgroundImage, 0, 0, panImage.ClientSize.Width, panImage.ClientSize.Height);
                    Bobber.Draw(g, 346, 157);
                    g.ResetTransform();
                    Bobber2.Draw(g, 45, 157);
                    List<Particle> removeparts = new List<Particle>();
                    foreach (Particle drawpart in DrawParticles)
                    {
                        if (drawpart.PerformFrame(null))
                            removeparts.Add(drawpart);
                    }
                    lock (DrawParticles)
                    {
                        foreach (Particle removeit in removeparts)
                        {
                            DrawParticles.Remove(removeit);
                        }
                    }
                    //BobberVersion.Draw(g, (int)g.MeasureString(BobberVersion.DrawString,BobberVersion.DrawFont).Width, 157);
                    panImage.Invoke((MethodInvoker)(() =>
                                                         {
                                                             panImage.Invalidate();
                                                             panImage.Update();
                                                         }));


                    //panImage.Update();
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Unexpected Exception (DrawFrame)" + ex.Message);

            }
        }

        private void AlphaTick(object value)
        {
            // Debug.Print(this.Opacity.ToString());
            this.Opacity += mAlphaIncrement;
            if (this.Opacity >= 1) mAlphaIncrement = 0;
            if (this.Opacity == 0)
            {
                Hide();
                // frmBaseBlock mbaseblockobj = new frmBaseBlock();
                //new frmBaseBlock().Show();
                //mbaseblockobj.Show();
                //mbaseblockobj.Closed += new EventHandler(mbaseblockobj_Closed);
                tmrFade.Enabled = false;

                BCBlockGameState.Soundman.StopMusic();
                Close(); //close the form.
            }
        }

        private void timercallback(object value)
        {
            //disable timer..
            Debug.Print("timercallback");
            delaytimer.Dispose();
            delaytimer = null;
            //invoke the routine to initialize the game state...
            if (!ShownAsAbout)
            {
                quickthread = new Thread(threadinitroutine);
                quickthread.Start();
            }
            bobberthread = new Thread(BobberThread);
            bobberthread.Start();
        }

        private void threadinitroutine()
        {
            System.Windows.Forms.Cursor.Current = Cursors.AppStarting;
            DateTime InitTime = DateTime.Now;
            BCBlockGameState.Initgamestate(this);
            //start playing the intro music

            String[] introkeys = BCBlockGameState.Soundman.getMultiSounds("INTRO");

            BCBlockGameState.Soundman.PlayMusic(introkeys, cNewSoundManager.MultiMusicPlayMode.MultiMusic_Order);
            Cursor.Current = Cursors.Default;
            mProceed = true;
            TimeSpan totalinittime = (DateTime.Now - InitTime);
            ShowMessage("Initialization completed in " + totalinittime);
            ShowMessage("Click Image above to continue...");
            
            //store that total init time in the initialization time log.
            TimeInitLog(totalinittime);


            //cmdCopy.Visible=true;
            cmdCopy.Invoke((MethodInvoker) (() => cmdCopy.Visible = true));
        }
        /// <summary>
        /// Adds the given Time to a list of Times. Stored in %APPDATA%, and that keep track of the startup times of the program, for "auditing" purposes, or something.
        /// </summary>
        /// <param name="Timelogged"></param>
        private static void TimeInitLog(TimeSpan Timelogged)
        {
            //first, what file will it be.
            String Initlogfile = Path.Combine(BCBlockGameState.AppDataFolder,"init.log");
            FileStream fs = new FileStream(Initlogfile, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("Version:" + GetExecutingVersion() + " - " + Timelogged.ToString());
            sw.Close();


        }

        private static String GetExecutingVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private String[] randomsel = BCBlockGameState.IntroStrings;
        private Random rgen = new Random();

        private String ChooseRandomBobText()
        {
            int randomindex = rgen.Next(0, randomsel.Length);

            return randomsel[randomindex];
        }

        private Color getrndcolor()
        {
            var hscolor = new HSLColor(rgen.NextDouble()*240, 240, 120);
            return hscolor;
        }

        private Control LowerPanel = null;

        private void ChangeBobText()
        {
            String randombobtext = ChooseRandomBobText();
            Bitmap measurebob = new Bitmap(1, 1);
            Font bobberfont = new Font("Arial", 72, FontStyle.Bold);
            Graphics mbob = Graphics.FromImage(measurebob);
            SizeF bobtextsize = mbob.MeasureString(randombobtext, bobberfont);
            Color RandomColor1 = getrndcolor();
            Color RandomColor2 = getrndcolor();
            float randomAngle = (float) (2*Math.PI*rgen.NextDouble());
            Brush bobtextbrush = new LinearGradientBrush(new RectangleF(0, 0, bobtextsize.Width, bobtextsize.Height),
                                                         RandomColor1, RandomColor2, randomAngle, true);
            Bobber =
                (Animatable)
                new BobbingTextAnimator(randombobtext, 35, bobberfont, 0.25f, 0.35f, 0.007f, -35, bobtextbrush);
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            //431, 260
            chooseimage();
            this.AllowTransparency = true;
            //first, initialize image panel to 431, 260
            panImage.Location = new Point(0, 0);
            //change this if the image is changed...
            panImage.Size = new Size(431, 260);

            //clear the background of the panImage to speed drawing
            //panImage.BackgroundImage=null;

            InitBackground();
            //ImageBackBufferCanvas.DrawImage(useBackground, 0, 0, BackgroundBitmap.Width, BackgroundBitmap.Height);
            //move the lower pane...

            panProgress.Visible = !ShownAsAbout;
            PanelAbout.Visible = ShownAsAbout;
            PanelAbout.Size = panProgress.Size;
            if (ShownAsAbout)
            {
                LowerPanel = PanelAbout;
                PanelAbout.Visible = true;
            }
            else
            {
                LowerPanel = panProgress;
            }
            LowerPanel.Visible = true;

            LowerPanel.Location = new Point(0, panImage.Bottom);
            this.Size = new Size(panImage.Width, LowerPanel.Bottom);
            //resize the textbox to show exactly two lines
            //panProgress.Height = txtprogress.Height;

            //this.ClientSize = new Size(this.ClientSize.Width,panImage.Height + panProgress.Height);
            this.Height = LowerPanel.Bottom + 5;

            //put in middle of the screen, as well.
            //get appropriate screen...
            Screen usescreen = Screen.FromHandle(this.Handle);
            //center on working area...
            Point uselocation = new Point(usescreen.WorkingArea.Width/2 - Width/2,
                                          usescreen.WorkingArea.Height/2 - Height/2);
            this.Location = uselocation;
            mAlphaIncrement = 0.01;
            this.Opacity = 100;

//            Bobber = new BobbingTextAnimator("BETA", 35, new Font("Arial", 72,FontStyle.Bold),0.25f,0.35f,0.007f,-35);
            //Bobber = new BobbingTextAnimator(GetExecutingVersion(), 35, new Font("Arial", 72, FontStyle.Bold), 0.25f, 0.35f, 0.007f, -35);
            String randombobtext = ChooseRandomBobText();
            Bitmap measurebob = new Bitmap(1, 1);
            Font bobberfont = new Font("Arial", 72, FontStyle.Bold);
            Graphics mbob = Graphics.FromImage(measurebob);
            SizeF bobtextsize = mbob.MeasureString(randombobtext, bobberfont);
            Color RandomColor1 = getrndcolor();
            Color RandomColor2 = getrndcolor();
            float randomAngle = (float) (2*Math.PI*rgen.NextDouble());
            Brush bobtextbrush = new LinearGradientBrush(new RectangleF(0, 0, bobtextsize.Width, bobtextsize.Height),
                                                         RandomColor1, RandomColor2, randomAngle, true);
            Bobber =
                (Animatable)
                new BobbingTextAnimator(randombobtext, 35, bobberfont, 0.25f, 0.35f, 0.007f, -35, bobtextbrush);
            Bobber2 =
                (Animatable)
                new BobbingTextAnimator(GetExecutingVersion(), 35, new Font("Arial", 64, FontStyle.Bold), 0.25f, 0.35f,
                                        0.007f, 32, new SolidBrush(Color.Yellow));
            //Bobber2 = (Animatable)new FallingTextAnimator(GetExecutingVersion(), new Font("Arial", 72, FontStyle.Bold), new PointF(5, 5), 
            //new SolidBrush(Color.Yellow), new Pen(Color.Black), new PointF(0, 5f), new RectangleF(panImage.ClientRectangle.Left, 
            //    panImage.ClientRectangle.Height, panImage.ClientRectangle.Width, panImage.ClientRectangle.Height));
            //BobberVersion = new BobbingTextAnimator(GetExecutingVersion(), 0, new Font("Segoe UI", 24, FontStyle.Bold), 1, 1, 0, 0);

            delaytimer = new Timer(timercallback, null, 0, 0);
            //alphatimer = new Timer(AlphaTick, null, 0, 10);
            tmrFade.Enabled = true;
            //BCBlockGameState.Initgamestate(this);
        }

        private void InitBackground()
        {
            BackgroundBitmap = new Bitmap(panImage.Size.Width, panImage.Size.Height);
            //draw the background pic to it...
            BackgroundBuffer = Graphics.FromImage(BackgroundBitmap);
            BackgroundBuffer.DrawImage(useBackground, 0, 0, BackgroundBitmap.Width, BackgroundBitmap.Height);
            ImageBackBufferbitmap = new Bitmap(panImage.Size.Width, panImage.Size.Height);
            ImageBackBufferCanvas = Graphics.FromImage(ImageBackBufferbitmap);
            ImageBackBufferCanvas.Clear(Color.Transparent);
        }

        private float CurrentProgress = 0;

        #region iManagerCallback Members

        public void UpdateProgress(float ProgressPercentage)
        {
            CurrentProgress = ProgressPercentage;
            Debug.Print("Progress set to " + ProgressPercentage);
        }

        public void ShowMessage(string message)
        {
            filelogwriter.ShowMessage(message);
            if (txtprogress.InvokeRequired)
            {
                txtprogress.Invoke((MethodInvoker) (() =>
                                                        {
                                                            txtprogress.Text += Environment.NewLine + message;
                                                            txtprogress.SelectionStart = txtprogress.Text.Length;
                                                            txtprogress.ScrollToCaret();
                                                        }));
            }
            else
            {
                {
                    txtprogress.Text += "\n" + message;
                    txtprogress.ScrollToCaret();
                }
            }

            #endregion
        }

        private void panImage_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(BackgroundBitmap, 0, 0);

            lock (DrawParticles)
            {
                foreach (Particle drawpart in DrawParticles)
                {
                    drawpart.Draw(e.Graphics);
                }
            }
            lock (imagesdraw)
            {
                foreach (drawimagedata loopdata in imagesdraw)
                {
                    // e.Graphics.DrawImage(loopdata.DrawImage, loopdata.Location);
                    loopdata.Draw(e.Graphics);
                }
            }

            e.Graphics.DrawImageUnscaled(ImageBackBufferbitmap, 0, 0);


            if (CurrentProgress < 1.0f && !ShownAsAbout)
            {
                //get the appropriate dimensions...
                float widthprogress = ImageBackBufferbitmap.Width*CurrentProgress;
                RectangleF drawprect = new RectangleF((float) widthprogress, 0,
                                                      ImageBackBufferbitmap.Width - widthprogress,
                                                      ImageBackBufferbitmap.Height);

                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Red)), drawprect);

                SizeF measuresize;
                String loadingtext = "Loading" + dupe('.', DateTime.Now.Millisecond/100%5);
                System.Drawing.Font loadingtextfont = new Font("Arabia", 48);
                measuresize = e.Graphics.MeasureString(loadingtext, loadingtextfont);

                RectangleF largerect = new RectangleF((float) panImage.ClientRectangle.Left,
                                                      (float) panImage.ClientRectangle.Top,
                                                      (float) panImage.ClientRectangle.Width,
                                                      (float) panImage.ClientRectangle.Height);
                RectangleF userect = BCBlockGameState.CenterRect(largerect, measuresize);
                GraphicsPath pusepath = new GraphicsPath();

                pusepath.AddString(loadingtext, loadingtextfont.FontFamily, (int) loadingtextfont.Style,
                                   loadingtextfont.Size, new PointF(48, 48), StringFormat.GenericDefault);

                e.Graphics.FillPath(new SolidBrush(Color.Yellow), pusepath);
                e.Graphics.DrawPath(new Pen(Color.Black, 2), pusepath);
            }
        }

        private String dupe(char character, int number)
        {
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < number; i++)
            {
                sb.Append(character);
            }
            return sb.ToString();
        }

        private void chooseimage()
        {
            //April 6th, birthday of windows 3.1

            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 6)
            {
                useBackground = panWin31.BackgroundImage;
            }
            else
            {
                //select a random start image from the available start images.
                Image[] selectfrom = new Image[] { panStandard.BackgroundImage, panStandard2.BackgroundImage,panStandard3.BackgroundImage };

                Image useimage = selectfrom[rgen.Next(selectfrom.Length)];



                useBackground = useimage;
            }


            //new as of July 14th 2011: colourize to a random hue 50% of the time.
            if (rgen.NextDouble() > 0.5d)
            {
                //choose a random colour...
                Color colourizeto = new HSLColor(rgen.NextDouble() * 240, 240, 120);
                ImageAttributes attribcolourize = new ImageAttributes();
                attribcolourize.SetColorMatrix(ColorMatrices.GetColourizer((float)(colourizeto.R)/255, (float)(colourizeto.G)/255,
                    (float)(colourizeto.B)/255));
                useBackground = BCBlockGameState.AppyImageAttributes(useBackground, attribcolourize);



            }


        }


        private void mbaseblockobj_Closed(object sender, EventArgs e)
        {
            //when the main form closes, close this splash as well.

            Close();
        }

        private void tmrFade_Tick(object sender, EventArgs e)
        {
            AlphaTick(null);
        }

        private void panImage_Click(object sender, EventArgs e)
        {
            if (mProceed)
            {
                //hide this form, load main form, and show it.
                mAlphaIncrement = -0.01;
            }
        }

        private void txtprogress_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtprogress_MouseEnter(object sender, EventArgs e)
        {
            //
        }

        private void txtprogress_MouseLeave(object sender, EventArgs e)
        {
            //
        }

        private void cmdCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtprogress.Text);
        }

        private void SplashScreen_Deactivate(object sender, EventArgs e)
        {
        }

        private void SplashScreen_Activated(object sender, EventArgs e)
        {
            LowerPanel.Visible = true;
        }

        private void ShowLoadedAssemblies(ListView showinlvw)
        {
            showinlvw.Items.Clear();
            showinlvw.Columns.Clear();
            showinlvw.View = View.Details;

            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            //add the columns
            showinlvw.Columns.Add("NAME", "Name");
            showinlvw.Columns.Add("PATH", "Full Path");
            showinlvw.Columns.Add("VERSION", "Version");

            foreach (Assembly loopassembly in loadedAssemblies)
            {
                String[] createdstring = new String[]
                                             {
                                                 loopassembly.GetName().Name, loopassembly.Location,
                                                 loopassembly.GetName().Version.ToString()
                                             };
                ListViewItem newitem = new ListViewItem(createdstring);
                newitem.Tag = loopassembly;
                showinlvw.Items.Add(newitem);
            }
        }

        private void chkShowLoaded_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowLoaded.Checked)
            {
                ShowLoadedAssemblies(lvwAssemblies);
            }
            lvwAssemblies.Visible = chkShowLoaded.Checked;
        }

        private void SplashScreen_Shown(object sender, EventArgs e)
        {
            LowerPanel.Visible = true;
            Debug.Print("LowerPanel Visible:" + LowerPanel.Visible.ToString());
        }

        private void SplashScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (quickthread != null && quickthread.IsAlive)
            {
                quickthread.Abort();
                quickthread = null;
            }
            if (bobberthread != null && bobberthread.IsAlive)
            {
                bobberthread.Abort();
                bobberthread = null;
            }
        }

        private void SplashScreen_KeyPress(object sender, KeyPressEventArgs e)
        {
        }


        private void SplashScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12 && e.Shift)
            {
                ChangeBobText();
            }
            else if (e.KeyCode == Keys.F12 && e.Control)
            {
                if(BCBlockGameState.Soundman!=null)
                    BCBlockGameState.Soundman.PauseMusic();
            }
            else if (e.KeyCode == Keys.F11)
            {
                chooseimage();
                InitBackground();


            }
        }
    }
}