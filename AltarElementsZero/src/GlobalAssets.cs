using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace AltarElementsZero.src
{
    sealed class GlobalAssets(
        GraphicsDevice graphicsDevice,
        GameServiceContainer gameServiceContainer) 
        : Assets(
            graphicsDevice: graphicsDevice,
            gameServiceContainer: gameServiceContainer)
    {
        public Texture2D? Placeholder { get; private set; }
        public Texture2D? RomanFont { get; private set; }
		public Texture2D? Atlas {  get; private set; }

        public Song? Level1OST {  get; private set; }
        public Song? BossOST { get; private set; }
        public Song? IntroOST { get; private set; }
        public Song? MenuOST { get; private set; }

        private SoundEffect? AttackDownSFX { get; set; }
        private SoundEffect? AttackSideSFX { get; set; }
        private SoundEffect? BreakingWallSFX { get; set; }
        private SoundEffect? BurningVineSFX { get; set; }
        private SoundEffect? CheckpointSFX { get; set; }
        private SoundEffect? CollectableSFX { get; set; }
		private SoundEffect? GrabSFX { get; set; }
		private SoundEffect? HitSFX { get; set; }
		private SoundEffect? JumpSFX { get; set; }
		private SoundEffect? LandingSFX { get; set; }
		private SoundEffect? LoadingScreenSFX { get; set; }
		private SoundEffect? MenuInSFX { get; set; }
        private SoundEffect? MenuOutSFX { get; set; }
		private SoundEffect? OraHitSFX { get; set; }
		private SoundEffect? PortalSFX { get; set; }
		private SoundEffect? SwitchOffSFX { get; set; }
		private SoundEffect? SwitchOnSFX { get; set; }
		private SoundEffect? ThrowSFX { get; set; }
		private SoundEffect? TokiShootSFX { get; set; }
		private SoundEffect? TorchOnSFX { get; set; }
		private SoundEffect? WaterInSFX { get; set; }

		public SoundEffectInstance? AttackDownSFXInstance { get; private set; }
		public SoundEffectInstance? AttackSideSFXInstance { get; private set; }
		public SoundEffectInstance? BreakingWallSFXInstance { get; private set; }
		public SoundEffectInstance? BurningVineSFXInstance { get; private set; }
		public SoundEffectInstance? CheckpointSFXInstance { get; private set; }
		public SoundEffectInstance? CollectableSFXInstance { get; private set; }
		public SoundEffectInstance? GrabSFXInstance { get; private set; }
		public SoundEffectInstance? HitSFXInstance { get; private set; }
		public SoundEffectInstance? JumpSFXInstance { get; private set; }
		public SoundEffectInstance? LandingSFXInstance { get; private set; }
		public SoundEffectInstance? LoadingScreenSFXInstance { get; private set; }
		public SoundEffectInstance? MenuInSFXInstance { get; private set; }
		public SoundEffectInstance? MenuOutSFXInstance { get; private set; }
		public SoundEffectInstance? OraHitSFXInstance { get; private set; }
		public SoundEffectInstance? PortalSFXInstance { get; private set; }
		public SoundEffectInstance? SwitchOffSFXInstance { get; private set; }
		public SoundEffectInstance? SwitchOnSFXInstance { get; private set; }
		public SoundEffectInstance? ThrowSFXInstance { get; private set; }
		public SoundEffectInstance? TokiShootSFXInstance { get; private set; }
		public SoundEffectInstance? TorchOnSFXInstance { get; private set; }
		public SoundEffectInstance? WaterInSFXInstance { get; private set; }



		public override void Load()
        {
            base.Load();

            Placeholder = _contentManager!.Load<Texture2D>("img/default_placeholder.png");
            RomanFont = _contentManager!.Load<Texture2D>("img/font_placeholder.png");
			Atlas = _contentManager!.Load<Texture2D>("img/atlas.png");


            Level1OST = Song.FromUri("", new Uri("assets/music/OST/ost_level1.ogg", UriKind.Relative));
            BossOST = Song.FromUri("", new Uri("assets/music/OST/ost_boss.ogg", UriKind.Relative));
            IntroOST = Song.FromUri("", new Uri("assets/music/OST/ost_intro.ogg", UriKind.Relative));
			MenuOST = Song.FromUri("", new Uri("assets/music/OST/ost_menu.ogg", UriKind.Relative));

			AttackDownSFX = _contentManager!.Load<SoundEffect>("music/SFX/attack_down.wav");
			AttackDownSFXInstance = AttackDownSFX.CreateInstance();
			AttackSideSFX = _contentManager!.Load<SoundEffect>("music/SFX/attack_side.wav");
			AttackSideSFXInstance = AttackSideSFX.CreateInstance();
			BreakingWallSFX = _contentManager!.Load<SoundEffect>("music/SFX/breaking_wall.wav");
			BreakingWallSFXInstance = BreakingWallSFX.CreateInstance();
			BurningVineSFX = _contentManager!.Load<SoundEffect>("music/SFX/burning_vine.wav");
			BurningVineSFXInstance = BurningVineSFX.CreateInstance();
			CheckpointSFX = _contentManager!.Load<SoundEffect>("music/SFX/checkpoint.wav");
			CheckpointSFXInstance = CheckpointSFX.CreateInstance();
			CollectableSFX = _contentManager!.Load<SoundEffect>("music/SFX/collectable.wav");
			CollectableSFXInstance = CollectableSFX.CreateInstance();
			GrabSFX = _contentManager!.Load<SoundEffect>("music/SFX/grab.wav");
			GrabSFXInstance = GrabSFX.CreateInstance();
			HitSFX = _contentManager!.Load<SoundEffect>("music/SFX/hit.wav");
			HitSFXInstance = HitSFX.CreateInstance();
			JumpSFX = _contentManager!.Load<SoundEffect>("music/SFX/jump.wav");
			JumpSFXInstance = JumpSFX.CreateInstance();
			LandingSFX = _contentManager!.Load<SoundEffect>("music/SFX/landing.wav");
			LandingSFXInstance = LandingSFX.CreateInstance();
			LoadingScreenSFX = _contentManager!.Load<SoundEffect>("music/SFX/loading_screen.wav");
			LoadingScreenSFXInstance = LoadingScreenSFX.CreateInstance();
			MenuInSFX = _contentManager!.Load<SoundEffect>("music/SFX/menu_in.wav");
			MenuInSFXInstance = MenuInSFX.CreateInstance();
			MenuOutSFX = _contentManager!.Load<SoundEffect>("music/SFX/menu_out.wav");
			MenuOutSFXInstance = MenuOutSFX.CreateInstance();
			OraHitSFX = _contentManager!.Load<SoundEffect>("music/SFX/ora_hit.wav");
			OraHitSFXInstance = OraHitSFX.CreateInstance();
			PortalSFX = _contentManager!.Load<SoundEffect>("music/SFX/portal.wav");
			PortalSFXInstance = PortalSFX.CreateInstance();
			SwitchOffSFX = _contentManager!.Load<SoundEffect>("music/SFX/switch_off.wav");
			SwitchOffSFXInstance = SwitchOffSFX.CreateInstance();
			SwitchOnSFX = _contentManager!.Load<SoundEffect>("music/SFX/switch_on.wav");
			SwitchOnSFXInstance = SwitchOnSFX.CreateInstance();
			ThrowSFX = _contentManager!.Load<SoundEffect>("music/SFX/throw.wav");
			ThrowSFXInstance = ThrowSFX.CreateInstance();
			TokiShootSFX = _contentManager!.Load<SoundEffect>("music/SFX/toki_shoot.wav");
			TokiShootSFXInstance = TokiShootSFX.CreateInstance();
			TorchOnSFX = _contentManager!.Load<SoundEffect>("music/SFX/torch_on.wav");
			TorchOnSFXInstance = TorchOnSFX.CreateInstance();
			WaterInSFX = _contentManager!.Load<SoundEffect>("music/SFX/water_in.wav");
			WaterInSFXInstance = WaterInSFX.CreateInstance();
		}

		public override void Unload()
        {
            base.Unload();

            Placeholder = null;
            RomanFont = null;
			Atlas = null;

            Level1OST = null;
            BossOST = null;
            IntroOST = null;
            MenuOST = null;

			AttackDownSFXInstance = null;
			AttackDownSFX = null;
			AttackSideSFXInstance = null;
			AttackSideSFX = null;
			BreakingWallSFXInstance = null;
			BreakingWallSFX = null;
			BurningVineSFXInstance = null;
			BurningVineSFX = null;
			CheckpointSFXInstance = null;
			CheckpointSFX = null;
			CollectableSFXInstance = null;
			CollectableSFX = null;
			GrabSFXInstance = null;
			GrabSFX = null;
			HitSFXInstance = null;
			HitSFX = null;
			JumpSFXInstance = null;
			JumpSFX = null;
			LandingSFXInstance	= null;
			LandingSFX = null;
			LoadingScreenSFXInstance = null;
			LoadingScreenSFX = null;
			MenuInSFXInstance = null;
			MenuInSFX = null;
			MenuOutSFXInstance = null;
			MenuOutSFX = null;
			OraHitSFXInstance = null;
			OraHitSFX = null;
			PortalSFXInstance = null;
			PortalSFX = null;
			SwitchOffSFXInstance = null;
			SwitchOffSFX = null;
			SwitchOnSFXInstance = null;
			SwitchOnSFX = null;
			ThrowSFXInstance = null;
			ThrowSFX = null;
			TokiShootSFXInstance = null;
			TokiShootSFX = null;
			TorchOnSFXInstance = null;
			TorchOnSFX = null;
			WaterInSFXInstance = null;
			WaterInSFX = null;

        }
    }
}
