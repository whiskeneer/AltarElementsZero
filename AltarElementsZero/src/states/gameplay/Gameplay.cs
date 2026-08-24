using AltarElementsZero.src.states.gameplay.gameObject;
using AltarElementsZero.src.states.gameplay.level;
using AltarElementsZero.src.states.gameplay.vectors;
using AltarElementsZero.src.renderer;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//using System;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.enemies;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.gimmicks;
using AltarElementsZero.src.states.intro;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.debug;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.player;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour;
using AltarElementsZero.src.screenEffects;
using AltarElementsZero.src.states.gameplay.cutscenes;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.triggers;
using AltarElementsZero.src.states.gameplay.gameObject.behaviour.bosses;
using AltarElementsZero.src.states.credits;
using Microsoft.Xna.Framework.Media;

namespace AltarElementsZero.src.states.gameplay
{
	[Flags]
	enum GameplayMessages : UInt32{
		None = 0,
		Exit = 1 << 0,
		RestartFromCheckpoint = 1 << 1,
		RestartFromBeginning = 1 << 2,
		Teleport = 1 << 3,
		Pause = 1 << 4,
		TriggerLevel1EndCutscene = 1 << 5,
		TriggerLevel1BossTransition = 1 << 6,
		TriggerLevel1BossEnd = 1 << 7,
	}

	interface ISignalFlags
	{
		// Flags for communication among GameObjects
		void SetSignalFlag(int flag, bool value);
		bool GetSignalFlag(int flag);

		// Flags for communicating with Gameplay
		void EmitGameplayMessage(GameplayMessages gameplayMessage);

		void SetCheckpoint(byte checkpointValue, TilePosition checkpointPosition);
		byte GetCheckpointValue();

		void SetTeleportDestiny(int chunkRelativeX,  int chunkRelativeY);

		SubpxPosition GetPlayerPosition();

		bool CreateGameObject(IBehaviour behaviour, byte spawnValue, SubpxPosition position);

		bool CreateAndAttachObject(IBehaviour behaviour, byte spawnValue, GameObject self);
		void GetChunkLimits(out uint limitUp, out uint limitDown, out uint limitLeft, out uint limitRight);

	}


    class Gameplay(
        GraphicsDevice graphicsDevice,
        GameServiceContainer gameServiceContainer,
        IManager manager,
        GameplayPayload payload,
        GlobalAssets globalAssets,
        InputHandler inputHandler,
		CutsceneManager cutsceneManager
        ) : State<GameplayAssets, GameplayPayload>(
            manager: manager,
            payload: payload,
            assets: new GameplayAssets(graphicsDevice, gameServiceContainer),
            inputHandler: inputHandler,
            globalAssets: globalAssets
            ), ISignalFlags
    {

		private CutsceneManager _cutsceneManager = cutsceneManager;

		private enum State {
			ENTERING_GAMEPLAY,
			PLAYING,
			PAUSED,
			RESUMING,
			GOING_TO_CHECKPOINT,
			RESTARTING,
			EXITING,

			GOING_TO_BOSS_STAGE,
			ENTERING_BOSS_STAGE,

			// Level 1 Boss
			PANNING_UP,
			WAITING_ABOVE,
			PANNING_DOWN,

			MERMAID_DEFEATED,
			LEVEL1_ENDING
		}
		private enum PauseOptions{
			RESUME,
			GO_TO_CHECKPOINT,
			RESTART,
			EXIT
		}

		private State state = State.ENTERING_GAMEPLAY;

		private PauseOptions selectedPauseOption = PauseOptions.RESUME;
		private int pauseFramePosition = 0;
		private int enteringPauseFramePosition = 128*4;
		private int enteringPauseOptionsPosition = 256*4;

		private int cutsceneTimer = 0;

		public bool CreateGameObject(IBehaviour behaviour, byte spawnValue, SubpxPosition position){
			int nextAssignableObject = GetNextAssignableObject();
			if (nextAssignableObject == -1) return false;

			GameObject newObject = _objectPool[nextAssignableObject];
			newObject.behaviour = behaviour;
			newObject.spawnValue = spawnValue;
			newObject.currentBoundingBox.Position = position;
			newObject.Type = GameObject.Types.SPAWNING;

			return true;

		}
		public bool CreateAndAttachObject(IBehaviour behaviour, byte spawnValue, GameObject self){
			int nextAssignableObject = GetNextAssignableObject();
			if (nextAssignableObject == -1) return false;

			GameObject newObject = _objectPool[nextAssignableObject];
			newObject.behaviour = behaviour;
			newObject.spawnValue = spawnValue;
			//newObject.currentBoundingBox.Position = position;
			newObject.Type = GameObject.Types.SPAWNING;
			self.linkedObject = newObject;


			//Console.WriteLine("CREATED");

			return true;
		}


		private GameplayMessages gameplayMessages = GameplayMessages.None;

		public void EmitGameplayMessage(GameplayMessages newGameplayMessage){
			gameplayMessages |= newGameplayMessage;
		}

		private void ProcessGameplayMessages()
		{
			if((gameplayMessages & GameplayMessages.Exit) == GameplayMessages.Exit)
			{
				_manager.RequestTransition(new IntroPayload("HELLO"));
			}
			else if ((gameplayMessages & GameplayMessages.RestartFromBeginning) == GameplayMessages.RestartFromBeginning)
			{
				RestartFromBeginning();
			}
			else if((gameplayMessages & GameplayMessages.RestartFromCheckpoint) == GameplayMessages.RestartFromCheckpoint)
			{
				RestartFromCheckpoint();
			}
			else if((gameplayMessages & GameplayMessages.Teleport) == GameplayMessages.Teleport)
			{
				Teleport();
				teleportChunkRelativeX = 0;
				teleportChunkRelativeY = 0;
			}
			else if ((gameplayMessages & GameplayMessages.TriggerLevel1EndCutscene) == GameplayMessages.TriggerLevel1EndCutscene)
			{
				_cutsceneManager.StartCutscene(CutsceneManager.CutsceneID.LEVEL1BOSS);
				MediaPlayer.Stop();
			}
			else if((gameplayMessages & GameplayMessages.TriggerLevel1BossTransition) == GameplayMessages.TriggerLevel1BossTransition)
			{
				state = State.GOING_TO_BOSS_STAGE;
				LoadingEffectStart.Instance.Start();

				_globalAssets!.LoadingScreenSFXInstance!.Stop();
				_globalAssets!.LoadingScreenSFXInstance!.Play();
			}
			else if ((gameplayMessages & GameplayMessages.TriggerLevel1BossEnd) == GameplayMessages.TriggerLevel1BossEnd)
			{
				cutsceneTimer = 0;
				state = State.MERMAID_DEFEATED;
				MediaPlayer.Stop();
			}
			else if ((gameplayMessages & GameplayMessages.Pause) == GameplayMessages.Pause)
			{
				if(_payload.Configuration == GameplayPayload.GameplayConfiguration.NORMAL_GAMEPLAY)
				{
					state = State.PAUSED;
					selectedPauseOption = PauseOptions.RESUME;
					enteringPauseFramePosition = 128 * 4;
					enteringPauseOptionsPosition = 256 * 4;


					_globalAssets!.MenuInSFXInstance!.Stop();
					_globalAssets!.MenuInSFXInstance!.Play();
				}
				else
				{
					_manager.RequestTransition(new IntroPayload("HELLO"));
				}
			}	

			//

			gameplayMessages = GameplayMessages.None;
		}

		public void SetCheckpoint(byte checkpointValue, TilePosition checkpointPosition){
			LastActivatedCheckpoint = checkpointPosition;
			LastActivatedCheckpointValue = checkpointValue;
		}
		public byte GetCheckpointValue(){
			return LastActivatedCheckpointValue;
		}

		private UInt32 PersistentSignalFlags4 = 0;
		private UInt32 PersistentSignalFlags3 = 0;
		private UInt32 PersistentSignalFlags2 = 0;
		private UInt32 PersistentSignalFlags1 = 0;
		private UInt32 SignalFlags4 = 0;
		private UInt32 SignalFlags3 = 0;
		private UInt32 SignalFlags2 = 0;
		private UInt32 SignalFlags1 = 0;

		private TilePosition BeginningCheckpoint = new(0, 0);
		private byte LastActivatedCheckpointValue = 0;
		private TilePosition LastActivatedCheckpoint = new(0, 0);

		private int teleportChunkRelativeX = 0;
		private int teleportChunkRelativeY = 0;

		public void SetTeleportDestiny(int chunkRelativeX, int chunkRelativeY)
		{
			teleportChunkRelativeX = chunkRelativeX;
			teleportChunkRelativeY = chunkRelativeY;
		}

		public void SetSignalFlag(int flag, bool value)
		{
			if (flag < 0 || flag > 255) return;

			if (flag < 32)
			{
				SignalFlags1 &= ~(1u << (int)flag);
				SignalFlags1 |= (value ? 1u : 0u) << (int)flag;
			}
			else if (flag < 64)
			{
				SignalFlags2 &= ~(1u << (int)(flag - 32));
				SignalFlags2 |= (value ? 1u : 0u) << (int)(flag - 32);
			}
			else if (flag < 96)
			{
				SignalFlags3 &= ~(1u << (int)(flag - 64));
				SignalFlags3 |= (value ? 1u : 0u) << (int)(flag - 64);
			}
			else if (flag < 128)
			{
				SignalFlags4 &= ~(1u << (int)(flag - 96));
				SignalFlags4 |= (value ? 1u : 0u) << (int)(flag - 96);
			}
			else if (flag < 160)
			{
				PersistentSignalFlags1 &= ~(1u << (int)(flag - 128));
				PersistentSignalFlags1 |= (value ? 1u : 0u) << (int)(flag - 128);
			}
			else if (flag < 192)
			{
				PersistentSignalFlags2 &= ~(1u << (int)(flag - 160));
				PersistentSignalFlags2 |= (value ? 1u : 0u) << (int)(flag - 160);
			}
			else if (flag < 224)
			{
				PersistentSignalFlags3 &= ~(1u << (int)(flag - 192));
				PersistentSignalFlags3 |= (value ? 1u : 0u) << (int)(flag - 192);
			}
			else
			{
				PersistentSignalFlags4 &= ~(1u << (int)(flag - 224));
				PersistentSignalFlags4 |= (value ? 1u : 0u) << (int)(flag - 224);
			}
		}

		public bool GetSignalFlag(int flag)
		{
			if (flag < 0 || flag > 255) return false;

			if (flag < 32)
			{
				return ((SignalFlags1 >> (int)flag) & 1) == 1;
			}
			else if (flag < 64)
			{
				return ((SignalFlags2 >> (int)(flag - 32)) & 1) == 1;
			}
			else if (flag < 96)
			{
				return ((SignalFlags3 >> (int)(flag - 64)) & 1) == 1;
			}
			else if (flag < 128)
			{
				return ((SignalFlags4 >> (int)(flag - 96)) & 1) == 1;
			}
			else if (flag < 160)
			{
				return ((PersistentSignalFlags1 >> (int)(flag - 128)) & 1) == 1;
			}
			else if (flag < 192)
			{
				return ((PersistentSignalFlags2 >> (int)(flag - 160)) & 1) == 1;
			}
			else if (flag < 224)
			{
				return ((PersistentSignalFlags3 >> (int)(flag - 192)) & 1) == 1;
			}
			else
			{
				return ((PersistentSignalFlags4 >> (int)(flag - 224)) & 1) == 1;
			}
		}



		private Level? _level; //= new("assets/lvl/DEBUG_LEVEL.json", "assets/lvl/DEBUG_LEVEL_CHUNKS.json");

        private bool frameByFrameMode = false;
		private bool stopCamera = false;
        private bool _drawIndices = false;

 
		private readonly GameObject[] _objectPool = new GameObject[64];

		public SubpxPosition GetPlayerPosition()
		{
			return _objectPool[0].currentBoundingBox.Center();
		}
		public bool IsPlayerEmittingLight()
		{
			return _objectPool[0].behaviour == Ora.Instance &&
				_objectPool[0].secondLinkedObject != null &&
				_objectPool[0].secondLinkedObject?.behaviour == Torch.Instance &&
				_objectPool[0].secondLinkedObject?.State == (uint)Torch.State.ON;
		}

		uint _animationFrame = 0;

		//

		private byte CurrentBackground = 0;
		private Force CurrentGravity = new(0, 12);
		private uint CurrentAirFriction = 0;

		private SubpxPosition PortalOutPosition = new(0, 0);

		private uint ChunkLimitTop = 0;
		private uint ChunkLimitBottom = 0;
		private uint ChunkLimitLeft = 0;
		private uint ChunkLimitRight = 0;
		public void GetChunkLimits(out uint limitUp, out uint limitDown, out uint limitLeft, out uint limitRight)
		{
			limitUp = ChunkLimitTop;
			limitDown = ChunkLimitBottom;
			limitLeft = ChunkLimitLeft;
			limitRight = ChunkLimitRight;
		}

		private PxPosition CameraPosition = new();

		private bool ApplyDarkness = false;
		private uint DarknessFrom = 0;
		private uint DarknessTo = uint.MaxValue;

		
		private int _assignableObjectAuxiliar = 0;
		private void ResetAssignableObjects()
		{
			_assignableObjectAuxiliar = 0;
		}
		private int GetNextAssignableObject()
		{
			if (_assignableObjectAuxiliar >= _objectPool.Length) return -1;

			for (int i = _assignableObjectAuxiliar; i < _objectPool.Length; i++)
			{
				if (_objectPool[i].Type == GameObject.Types.NONEXISTENT)
				{
					_assignableObjectAuxiliar = i + 1;
					return i;
				}
			}

			_assignableObjectAuxiliar = _objectPool.Length;
			return -1;

		}

		private void UpdateChunk(Chunk chunk)
		{
			if (chunk.BackgroundIndex == 0) return;

			CurrentBackground = chunk.BackgroundIndex;
			switch(CurrentBackground){
				case 3: // underwater
				case 0x12:
					CurrentGravity = Configuration.WaterGravity;
					CurrentAirFriction = Configuration.WaterFriction;
					break;
				default:
					CurrentGravity = Configuration.AirGravity;
					CurrentAirFriction = Configuration.AirFriction;
					break;
			}


			ChunkLimitTop = (uint)(chunk.Top) * (uint)Configuration.Chunk.Subpx.Height;
			ChunkLimitBottom = (uint)(chunk.Bottom + 1) * (uint)Configuration.Chunk.Subpx.Height - 1;
			ChunkLimitLeft = (uint)(chunk.Left) * (uint)Configuration.Chunk.Subpx.Width;
			ChunkLimitRight = (uint)(chunk.Right + 1) * (uint)Configuration.Chunk.Subpx.Width - 1;

			PortalOutPosition = new SubpxPosition(ChunkLimitLeft, ChunkLimitTop);

			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject go = _objectPool[o];
				if(go.Type != GameObject.Types.NONEXISTENT && go.isPersistentAcrossChunks == false)
				{
					go.behaviour = EmptyObject.Instance;
					go.Init();
				}
			}

			SignalFlags1 = 0;
			SignalFlags2 = 0;
			SignalFlags3 = 0;
			SignalFlags4 = 0;

			ResetAssignableObjects();
			bool objectPoolIsFull = false;
			for(int j = chunk.Top * Configuration.Chunk.Tile.Height;
				!objectPoolIsFull &&
				j <= (chunk.Bottom + 1) * Configuration.Chunk.Tile.Height - 1;
				j++)
			{
				for (int i = chunk.Left * Configuration.Chunk.Tile.Width;
					!objectPoolIsFull &&
					i <= (chunk.Right + 1) * Configuration.Chunk.Tile.Width - 1;
					i++)
				{
					Tile tile = _level!.GetTile(i, j);

					if (tile.IsObjectSpawn())
					{
						int nextAssignableObject = GetNextAssignableObject();
						if(nextAssignableObject == -1)
						{
							objectPoolIsFull = true;
							break;
						}

						// Is there a way to optimize this? 
						// (for instance: indexed functions instead of long else-if chain)

						IBehaviour spawningObjectBehaviour = tile.Family switch
						{
							Tile.Families.Toki => Toki.Instance,
							Tile.Families.MovingPlatform1 => MovingPlatform1.Instance,
							Tile.Families.DebugBox => DebugBox.Instance,
							Tile.Families.DebugPusher => DebugPusher.Instance,
							Tile.Families.DebugImmobile => DebugImmobile.Instance,
							Tile.Families.FanUp => CurrentUp.Instance,
							Tile.Families.FloorButton => FloorButton.Instance,
							Tile.Families.SwitchableDoor => SwitchableDoor.Instance,
							Tile.Families.Checkpoint => Checkpoint.Instance,
							Tile.Families.TurbineLeft => TurbineCurrentLeft.Instance,
							Tile.Families.TurbineRight => TurbineCurrentRight.Instance,
							Tile.Families.Ufo => Ufo.Instance,
							Tile.Families.BreakableTile => BreakableTile.Instance,
							Tile.Families.Fire => Fire.Instance,
							Tile.Families.Torch => Torch.Instance,
							Tile.Families.Vine => Vine.Instance,
							Tile.Families.Darkness => Darkness.Instance,
							Tile.Families.Water => WaterRegion.Instance,
							Tile.Families.ClockKey => ClockKey.Instance,
							Tile.Families.PortalIn => Portal.Instance,
							Tile.Families.Level1EndTrigger => Level1EndTrigger.Instance,
							Tile.Families.Level1EndFloor => FloorLevel1EndCutscene.Instance,
							Tile.Families.Mermaid => Mermaid.Instance,
							Tile.Families.Boss1MovingPlatform => Boss1MovingPlatform.Instance,
							Tile.Families.MovingSpringHorizontal => MovingSpringHorizontal.Instance,
							Tile.Families.MovingSpringVertical => MovingSpringVertical.Instance,
							_ => EmptyObject.Instance,
						};

						if (tile.Family == Tile.Families.PortalOut)
						{
							PortalOutPosition = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
						}

						if(spawningObjectBehaviour != EmptyObject.Instance)
						{
							_objectPool[nextAssignableObject].behaviour = spawningObjectBehaviour;
							_objectPool[nextAssignableObject].spawnValue = tile.Member;
							_objectPool[nextAssignableObject].currentBoundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							_objectPool[nextAssignableObject].Init();
						}

						if(tile.Family == Tile.Families.Barrel)
						{
							_objectPool[nextAssignableObject].behaviour = Barrel.Instance;
							_objectPool[nextAssignableObject].spawnValue = tile.Member;


							nextAssignableObject = GetNextAssignableObject();
							if (nextAssignableObject == -1)
							{
								objectPoolIsFull = true;
								break;
							}
							_objectPool[nextAssignableObject].behaviour = BarrelTop.Instance;
							_objectPool[nextAssignableObject].spawnValue = tile.Member;

							_objectPool[nextAssignableObject - 1].linkedObject = _objectPool[nextAssignableObject];

							_objectPool[nextAssignableObject-1].currentBoundingBox.Position = (new TilePosition((uint)i, (uint)j).ToPx() + new PxPosition(5,0)).ToSubpx();
							_objectPool[nextAssignableObject].currentBoundingBox.Position = new TilePosition((uint)i, (uint)j).ToPx().ToSubpx();
							_objectPool[nextAssignableObject].Init();
							_objectPool[nextAssignableObject-1].Init();
						}
					}
				}
			}
		}
		private void UpdateCamera(SubpxPosition focusPosition)
		{
			PxPosition focusPxPosition = focusPosition.ToPx();
			int targetY = (int)focusPxPosition.Y - (Configuration.VisibleScreen.Px.Height >> 1);
			int targetX = (int)focusPxPosition.X - (Configuration.VisibleScreen.Px.Width >> 1);

			int visualLimitTop = (int)(ChunkLimitTop >> Configuration.Px.SubpxPower);
			int visualLimitBottom =
				(int)((ChunkLimitBottom + 1) >> Configuration.Px.SubpxPower) - Configuration.VisibleScreen.Px.Height;
			int visualLimitLeft = (int)(ChunkLimitLeft >> Configuration.Px.SubpxPower);
			int visualLimitRight =
				(int)((ChunkLimitRight + 1) >> Configuration.Px.SubpxPower) - Configuration.VisibleScreen.Px.Width;

			if (targetY < visualLimitTop) targetY = visualLimitTop;
			if (targetY > visualLimitBottom) targetY = visualLimitBottom;
			if (targetX < visualLimitLeft) targetX = visualLimitLeft;
			if (targetX > visualLimitRight) targetX = visualLimitRight;

			CameraPosition = new PxPosition((uint)targetX, (uint)targetY);
		}



		//

		private void LoadLevel(string tilesFileName, string chunksFileName)
		{
			_level = new Level(tilesFileName, chunksFileName);
		}
		private bool StartLevel()
		{
			if (_level == null) return false;

			for (int o = 0; o < _objectPool.Length; o++)
			{
				_objectPool[o] = new();
				GameObject go = _objectPool[o];
				go.behaviour = EmptyObject.Instance;
				go.Init();
			}

			bool foundPlayer = false;

			for (int j = 0; j < Configuration.Level.Tile.Height && !foundPlayer; j++)
			{
				for (int i = 0; i < Configuration.Level.Tile.Width && !foundPlayer; i++)
				{
					Tile tile = _level.GetTile(i, j);
					if(tile.Family == Tile.Families.Ora){
						foundPlayer = true;

						BeginningCheckpoint = new TilePosition((uint)i, (uint)j);
						RestartFromBeginning();

						MediaPlayer.Stop();
						MediaPlayer.IsRepeating = true;
						MediaPlayer.Play(_globalAssets.Level1OST);
					}
				}
			}

			return foundPlayer;
		}

		private void Teleport()
		{
			GameObject player = _objectPool[0];
			ChunkPosition currentChunk = player.currentBoundingBox.Center().ToPx().ToTile().ToChunk();
			ChunkPosition destinyChunk = new((uint)(currentChunk.X + teleportChunkRelativeX), (uint)(currentChunk.Y + teleportChunkRelativeY));

			UpdateChunk(_level!.GetChunk((int)destinyChunk.X, (int)destinyChunk.Y));
			player.currentBoundingBox.Position = PortalOutPosition;
			player.previousBoundingBox.Position = PortalOutPosition;
			UpdateCamera(player.currentBoundingBox.Center());
		}

		private void RestartFromCheckpoint(){


			GameObject player = _objectPool[0];
			player.Delete();
			player.behaviour = Ora.Instance;
			player.currentBoundingBox.Position = LastActivatedCheckpoint.ToPx().ToSubpx();
			player.Init();

			GameObject scythe = _objectPool[1];
			scythe.Delete();
			scythe.behaviour = Scythe.Instance;
			scythe.Init();
			player.LinkWith(scythe);


			int chunkX = ((int)LastActivatedCheckpoint.X / Configuration.Chunk.Tile.Width);
			int chunkY = ((int)LastActivatedCheckpoint.Y / Configuration.Chunk.Tile.Height);

			UpdateChunk(_level!.GetChunk(chunkX, chunkY));
			UpdateCamera(player.currentBoundingBox.Center());

			//
			if(LastActivatedCheckpointValue == 0 && _payload.Configuration == GameplayPayload.GameplayConfiguration.NORMAL_GAMEPLAY)
			{

				_cutsceneManager.StartCutscene(CutsceneManager.CutsceneID.LEVEL1START);

				
			}
			//

		}
		private void RestartFromBeginning(){
			LastActivatedCheckpoint = BeginningCheckpoint;
			LastActivatedCheckpointValue = 0;
			PersistentSignalFlags1 = 0;
			PersistentSignalFlags2 = 0;
			PersistentSignalFlags3 = 0;
			PersistentSignalFlags4 = 0;
			RestartFromCheckpoint();
		}



        public override void Enter()
        {
            GameObject.inputHandler = _inputHandler;
			GameObject.globalAssets = _globalAssets;
			GameObject.signalFlags = this;

			_cutsceneManager.ResetCutscenes();

            base.Enter();

			switch(_payload.Configuration)
			{
				case GameplayPayload.GameplayConfiguration.NORMAL_GAMEPLAY:
					break;
				case GameplayPayload.GameplayConfiguration.RECORD_AUTOPLAY:
					_inputHandler.StartRecording();
					break;
				case GameplayPayload.GameplayConfiguration.PLAY_AUTOPLAY:
					_inputHandler.StartPlayback();
					break;
			}

			LoadLevel("assets/lvl/DEBUG_LEVEL.json", "assets/lvl/DEBUG_LEVEL_CHUNKS.json");
			if (!StartLevel()){
				_manager.RequestTransition(new IntroPayload("ERROR"));
			}

			state = State.ENTERING_GAMEPLAY;
			LoadingEffectEnd.Instance.Start();


        }
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			ApplyDarkness = false;
			DarknessFrom = 0;
			DarknessTo = uint.MaxValue;

			for(int o = 0; o < _objectPool.Length; o++){
				if(_objectPool[o].Type == GameObject.Types.SPAWNING){
					_objectPool[o].Init();
				}
				if(_objectPool[o].behaviour == Darkness.Instance)
				{
					ApplyDarkness = true;
					Darkness.GetSpan(_objectPool[o], out DarknessFrom, out DarknessTo);
				}
			}

			if(state == State.ENTERING_GAMEPLAY)
			{
				LoadingEffectEnd.Instance.Update();
				if(LoadingEffectEnd.Instance.IsFinished())
				{
					state = State.PLAYING;
				}
			}
			else if(state == State.ENTERING_BOSS_STAGE)
			{ 
				LoadingEffectEnd.Instance.Update();
				if(LoadingEffectEnd.Instance.IsFinished())
				{
					//state = State.PLAYING;
					state = State.PANNING_UP;
				}
			}
			else if(state == State.PANNING_UP)
			{
				CameraPosition.Y -= 2;
				if(CameraPosition.Y <= (ChunkLimitTop >> Configuration.Px.SubpxPower))
				{
					state = State.WAITING_ABOVE;
					//Console.WriteLine("WAITING ABOVE");
					cutsceneTimer = 0;

				}
			}
			else if(state == State.WAITING_ABOVE)
			{
				cutsceneTimer++;
				if(cutsceneTimer >= 90)
				{
					state = State.PANNING_DOWN;
				}
			}
			else if(state == State.PANNING_DOWN)
			{
				CameraPosition.Y += 2;
				//if (CameraPosition.Y >= (ChunkLimitTop >> Configuration.Px.SubpxPower))
				if(CameraPosition.Y >= (((int)_objectPool[0].currentBoundingBox.Center().ToPx().Y - (Configuration.VisibleScreen.Px.Height >> 1))))
				{
					Ora.Instance.ContinueReadingInput(_objectPool[0]);
					state = State.PLAYING;

					MediaPlayer.Play(_globalAssets.BossOST);

				}
			}
			else if(state == State.MERMAID_DEFEATED)
			{
				cutsceneTimer++;
				for(int o = 0;  o < _objectPool.Length; o++)
				{ 
					if(_objectPool[o].behaviour == Mermaid.Instance)
					{
						_objectPool[o].behaviour.Update(_objectPool[o]);
					}
				}

				if(cutsceneTimer == 90)
				{
					LoadingEffectStart.Instance.Start();

					_globalAssets!.LoadingScreenSFXInstance!.Stop();
					_globalAssets!.LoadingScreenSFXInstance!.Play();
				}
				else if(cutsceneTimer >= 90)
				{
					LoadingEffectStart.Instance.Update();
					if(LoadingEffectStart.Instance.IsFinished())
					{
						cutsceneTimer = 0;
						LoadingEffectEnd.Instance.Start();
						state = State.LEVEL1_ENDING;
					}
				}

			}
			else if(state == State.LEVEL1_ENDING)
			{
				if(cutsceneTimer == 0)
				{
					LoadingEffectEnd.Instance.Update();
					if (LoadingEffectEnd.Instance.IsFinished())
					{
						cutsceneTimer = 1;
					}
				}
				else
				{
					if (_cutsceneManager.IsPlayingACutscene())
					{
						_cutsceneManager.Update();
					}
					else
					{
						cutsceneTimer++;

						if(cutsceneTimer == 60)
						{
							_cutsceneManager.StartCutscene(CutsceneManager.CutsceneID.LEVEL1END);
						}
						if(cutsceneTimer == 120)
						{
							_manager.RequestTransition(new CreditsPayload());
						}
					}
				}

			}
			else if(state == State.PLAYING)
			{
				if(_cutsceneManager.IsPlayingACutscene())
				{
					_cutsceneManager.Update();
				}
				else
				{
					ResetAssignableObjects();

					CalculateDesiredOutcomes();
					ApplyHorizontalVelocities();
					CheckHorizontalCollisions();
					ApplyVerticalVelocities();
					CheckVerticalCollisions();
					SeparatePushables();
					SeparatePushablesFromImmobile();
					FindCameraTarget();
					ProcessGameplayMessages();
				}
			}
			else if(state == State.RESUMING)
			{
				pauseFramePosition++;
				if (pauseFramePosition >= 2 * 3 * 16)
				{
					pauseFramePosition = 0;
				}
				enteringPauseFramePosition += Math.Max(1, enteringPauseFramePosition >> 2);
				enteringPauseOptionsPosition += Math.Max(1,enteringPauseOptionsPosition >> 2);

				if(enteringPauseOptionsPosition > 256 * 4)
				{
					state = State.PLAYING;
				}
			}
			else if(state == State.PAUSED)
			{

				pauseFramePosition++;
				if(pauseFramePosition >= 2 * 3 * 16){
					pauseFramePosition = 0;
				}
				enteringPauseFramePosition = Math.Max(0, enteringPauseFramePosition - Math.Max((enteringPauseFramePosition>>2),1));
				enteringPauseOptionsPosition = Math.Max(0, enteringPauseOptionsPosition - Math.Max((enteringPauseOptionsPosition >> 2), 1));

				if (_inputHandler.IsPressed(Input.Up))
				{
					selectedPauseOption--;
					if(selectedPauseOption < PauseOptions.RESUME)
					{
						selectedPauseOption = PauseOptions.EXIT;
					}
				}
				if(_inputHandler.IsPressed(Input.Down))
				{
					selectedPauseOption++;
					if(selectedPauseOption > PauseOptions.EXIT)
					{
						selectedPauseOption = PauseOptions.RESUME;
					}
				}

				if(_inputHandler.IsPressed(Input.Jump) || _inputHandler.IsPressed(Input.Pause))
				{
					switch(selectedPauseOption){
						case PauseOptions.RESUME:
							state = State.RESUMING;
							enteringPauseFramePosition = 4;
							enteringPauseOptionsPosition = 4;

							_globalAssets!.MenuOutSFXInstance!.Stop();
							_globalAssets!.MenuOutSFXInstance!.Play();

							break;
						case PauseOptions.GO_TO_CHECKPOINT:
							//state = State.PLAYING;
							//RestartFromCheckpoint();
							state = State.GOING_TO_CHECKPOINT;
							LoadingEffectStart.Instance.Start();

							_globalAssets!.LoadingScreenSFXInstance!.Stop();
							_globalAssets!.LoadingScreenSFXInstance!.Play();
							break;
						case PauseOptions.RESTART:
							//	state = State.PLAYING;
							//	RestartFromBeginning();
							state = State.RESTARTING;
							LoadingEffectStart.Instance.Start();

							_globalAssets!.LoadingScreenSFXInstance!.Stop();
							_globalAssets!.LoadingScreenSFXInstance!.Play();
							break;
						case PauseOptions.EXIT:
							//_manager.RequestTransition(new IntroPayload("HELLO"));
							state = State.EXITING;
							LoadingEffectStart.Instance.Start();

							_globalAssets!.LoadingScreenSFXInstance!.Stop();
							_globalAssets!.LoadingScreenSFXInstance!.Play();
							break;
					}
				}

			}
			else if(state == State.GOING_TO_CHECKPOINT)
			{
				LoadingEffectStart.Instance.Update();
				if(LoadingEffectStart.Instance.IsFinished())
				{
					RestartFromCheckpoint();
					state = State.ENTERING_GAMEPLAY;
					LoadingEffectEnd.Instance.Start();
				}
			}
			else if(state == State.RESTARTING)
			{
				LoadingEffectStart.Instance.Update();
				if (LoadingEffectStart.Instance.IsFinished())
				{
					RestartFromBeginning();
					state = State.ENTERING_GAMEPLAY;
					LoadingEffectEnd.Instance.Start();
				}
			}
			else if(state == State.GOING_TO_BOSS_STAGE)
			{
				LoadingEffectStart.Instance.Update();
				if (LoadingEffectStart.Instance.IsFinished())
				{
					Teleport();
					SetCheckpoint(0xff, _objectPool[0].currentBoundingBox.Position.ToPx().ToTile());
					//CameraPosition += new PxPosition(0,(uint)Configuration.Chunk.Px.Height);
					CameraPosition.Y = (ChunkLimitBottom >> Configuration.Px.SubpxPower) - (uint)Configuration.Chunk.Px.Height;
					state = State.ENTERING_BOSS_STAGE;
					LoadingEffectEnd.Instance.Start();

				}
			}
			else if(state== State.EXITING)
			{
				LoadingEffectStart.Instance.Update();
				if (LoadingEffectStart.Instance.IsFinished())
				{
					_manager.RequestTransition(new IntroPayload("HELLO"));
					MediaPlayer.Stop();
				}

			}


		}

        private void FindCameraTarget()
		{
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject go = _objectPool[o];
				if (!stopCamera)
				{
					if (object.ReferenceEquals(go.behaviour, DebugPusher.Instance) || object.ReferenceEquals(go.behaviour, Ora.Instance))
					{
						SubpxPosition focusCenter = go.currentBoundingBox.Center();

						if ((int)focusCenter.X < (int)ChunkLimitLeft ||
							(int)focusCenter.X > (int)ChunkLimitRight ||
							(int)focusCenter.Y < (int)ChunkLimitTop ||
							(int)focusCenter.Y > (int)ChunkLimitBottom
							)
						{// focus is outside of chunk!
							ChunkPosition newChunk = focusCenter.ToPx().ToTile().ToChunk();
							UpdateChunk(_level!.GetChunk((int)newChunk.X, (int)newChunk.Y));
						}

						UpdateCamera(focusCenter);
					}
				}
			}
		}
        private void CalculateDesiredOutcomes()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject gameObject = _objectPool[o];
				if (gameObject.Type != GameObject.Types.NONEXISTENT && gameObject.Type != GameObject.Types.SPAWNING)
				{
					gameObject.SavePreviousValues();
					gameObject.CalculateDesiredOutcome();

					gameObject.VelocityBelow = 0;
					gameObject.FrictionCoefficientsBelow = new();
					gameObject.VelocityAround = new();
					gameObject.FrictionCoefficientAround = CurrentAirFriction;
					gameObject.Gravity = CurrentGravity;

					gameObject.AppliedForces = new();

				}
			}
		}

        private void ApplyHorizontalVelocities()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject gameObject = _objectPool[o];
				if (gameObject.Type != GameObject.Types.NONEXISTENT && gameObject.Type != GameObject.Types.RESERVED && gameObject.Type != GameObject.Types.SPAWNING)
				{
					gameObject.ApplyHorizontalDesiredVelocity();

					gameObject.linkedObject?.currentBoundingBox.Position = gameObject.currentBoundingBox.Position + gameObject.linkedPosition;
					gameObject.secondLinkedObject?.currentBoundingBox.Position = gameObject.currentBoundingBox.Position + gameObject.secondLinkedPosition;
				}
			}
		}

        private void CheckHorizontalCollisions()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject go1 = _objectPool[o];

				

				if (go1.Type == GameObject.Types.NONEXISTENT || go1.Type == GameObject.Types.RESERVED || go1.Type == GameObject.Types.SPAWNING) continue;
				go1.CleanHorizontalPushFlags();


				//if (go1.Type != GameObject.Types.PUSHABLE && go1.Type != GameObject.Types.PROJECTILE) continue;

				if (go1.Type == GameObject.Types.PUSHABLE || go1.Type == GameObject.Types.PROJECTILE){

					TileSpan tileSpan = go1.currentBoundingBox.GetTileSpan();
					if(go1.currentVelocity.X > 0)
					{ // going right
						bool foundCollision = false;

						for(int col = (int)tileSpan.Left;
							col <= (int)tileSpan.Right && !foundCollision;
							col++)
						{
							bool oraFoundSpikes = false;
							bool oraFoundWall = false;

							for(int row  = (int)tileSpan.Top;
								row <= (int)tileSpan.Bottom && !foundCollision;
								row++)
							{
								Tile tile = _level.GetTile(col,row);
								if (!tile.IsSolid()) continue;

								ObjectBoundingBox tileBoundingBox = ObjectBoundingBox.FromTile((uint)col, (uint)row);
								if (go1.currentBoundingBox & tileBoundingBox)
								{




									// TODO: que sea una funcion

									if(object.ReferenceEquals(go1.behaviour,Ora.Instance) && tile.Family == Tile.Families.Spikes)
									{
										//go1.InteractionFlags |= (uint)(Ora.FlagTypes.Hurt);
										oraFoundSpikes = true;
									}
									else
									{
										go1.currentBoundingBox.LeanAtLeft(tileBoundingBox, (uint)Configuration.Tile.Subpx.Width);
										go1.FixHorizontalVelocity();
										go1.PushedLeft = true;

										oraFoundWall = true;
										foundCollision = true;
									}
								}



							}

							if(oraFoundSpikes && !oraFoundWall)
							{
								go1.InteractionFlags |= (uint)(Ora.FlagTypes.Hurt);
							}

						}
					}
					else
					{ // going left (or idle)
						bool foundCollision = false;

						for (int col = (int)tileSpan.Right;
							col >= (int)tileSpan.Left && !foundCollision;
							col--)
						{
							bool oraFoundSpikes = false;
							bool oraFoundWall = false;

							for (int row = (int)tileSpan.Top;
								row <= (int)tileSpan.Bottom && !foundCollision;
								row++)
							{
								Tile tile = _level.GetTile(col, row);
								if (!tile.IsSolid()) continue;

								ObjectBoundingBox tileBoundingBox = ObjectBoundingBox.FromTile((uint)col, (uint)row);
								if (go1.currentBoundingBox & tileBoundingBox)
								{

									//foundCollision = true;

									if (object.ReferenceEquals(go1.behaviour, Ora.Instance) && tile.Family == Tile.Families.Spikes)
									{
										//go1.InteractionFlags |= (uint)(Ora.FlagTypes.Hurt);
										oraFoundSpikes = true;
									}
									else
									{
										go1.currentBoundingBox.LeanAtRight(tileBoundingBox, (uint)Configuration.Tile.Subpx.Width);
										go1.FixHorizontalVelocity();
										go1.PushedRight = true;

										oraFoundWall = true;
										foundCollision = true;
									}
								}

							}

							if (oraFoundSpikes && !oraFoundWall)
							{
								go1.InteractionFlags |= (uint)(Ora.FlagTypes.Hurt);
							}
						}
					}


					//if (o == 0)
					//{ // ORA
					//	GameObject linked = _objectPool[o + 1]; // SCYTHE
					//	linked.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.linkedPosition;
					//}

					go1.linkedObject?.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.linkedPosition;
					go1.secondLinkedObject?.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.secondLinkedPosition;


				}

				for (int u = o + 1; u < _objectPool.Length; u++)
				{
					GameObject go2 = _objectPool[u];
					if (go2.Type == GameObject.Types.NONEXISTENT || go2.Type == GameObject.Types.RESERVED || go2.Type == GameObject.Types.SPAWNING) continue;

					GameObject.CheckHorizontalCollisions(go1, go2);
				}


			}
		}

        private void ApplyVerticalVelocities()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject gameObject = _objectPool[o];
				if (gameObject.Type != GameObject.Types.NONEXISTENT && gameObject.Type != GameObject.Types.RESERVED && gameObject.Type != GameObject.Types.SPAWNING)
				{
					gameObject.ApplyVerticalDesiredVelocity();

					gameObject.linkedObject?.currentBoundingBox.Position = gameObject.currentBoundingBox.Position + gameObject.linkedPosition;
					gameObject.secondLinkedObject?.currentBoundingBox.Position = gameObject.currentBoundingBox.Position + gameObject.secondLinkedPosition;
				}
			}
		}

        private void CheckVerticalCollisions()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject go1 = _objectPool[o];
				if (go1.Type == GameObject.Types.NONEXISTENT || go1.Type == GameObject.Types.RESERVED || go1.Type == GameObject.Types.SPAWNING) continue;
				go1.CleanVerticalPushFlags();


				//if (go1.Type != GameObject.Types.PUSHABLE && go1.Type != GameObject.Types.PROJECTILE) continue;

				if (go1.Type == GameObject.Types.PUSHABLE || go1.Type == GameObject.Types.PROJECTILE)
				{
					TileSpan tileSpan = go1.currentBoundingBox.GetTileSpan();
					if (go1.currentVelocity.Y > 0)
					{ // going down
						bool foundCollision = false;

						for (int row = (int)tileSpan.Top;
							row <= (int)tileSpan.Bottom && !foundCollision;
							row++)
						{

							bool oraFoundSpikes = false;
							bool oraFoundWall = false;

							for (int col = (int)tileSpan.Left;
								col <= (int)tileSpan.Right && !foundCollision;
								col++)
							{
								Tile tile = _level.GetTile(col, row);
								if (!tile.IsSolid()) continue;

								ObjectBoundingBox tileBoundingBox = ObjectBoundingBox.FromTile((uint)col, (uint)row);
								if (go1.currentBoundingBox & tileBoundingBox)
								{


									if (object.ReferenceEquals(go1.behaviour, Ora.Instance) && tile.Family == Tile.Families.Spikes)
									{
										//go1.InteractionFlags |= (uint)(Ora.FlagTypes.Hurt);
										oraFoundSpikes = true;
									}
									else
									{
										go1.currentBoundingBox.LeanAbove(tileBoundingBox, (uint)Configuration.Tile.Subpx.Height);
										go1.FixVerticalVelocity();
										go1.PushedUp = true;
										//foundCollision = true;

										go1.VelocityBelow = tile.GetSurfaceVelocityAbove();
										go1.FrictionCoefficientsBelow = tile.GetFrictionCoefficients();

										oraFoundWall = true;
										foundCollision = true;
									}

								}

							}

							if (oraFoundSpikes && !oraFoundWall)
							{
								go1.InteractionFlags |= (uint)(Ora.FlagTypes.Hurt);
							}
						}
					}
					else
					{ // going up (or idle)
						bool foundCollision = false;

						for (int row = (int)tileSpan.Bottom;
							row >= (int)tileSpan.Top && !foundCollision;
							row--)
						{

							bool oraFoundSpikes = false;
							bool oraFoundWall = false;


							for (int col = (int)tileSpan.Left;
								col <= (int)tileSpan.Right && !foundCollision;
								col++)
							{
								Tile tile = _level.GetTile(col, row);
								if (!tile.IsSolid()) continue;

								ObjectBoundingBox tileBoundingBox = ObjectBoundingBox.FromTile((uint)col, (uint)row);
								if (go1.currentBoundingBox & tileBoundingBox)
								{

									//foundCollision = true;

									if (object.ReferenceEquals(go1.behaviour, Ora.Instance) && tile.Family == Tile.Families.Spikes)
									{
										//go1.InteractionFlags |= (uint)(Ora.FlagTypes.Hurt);
										oraFoundSpikes = true;
									}
									else
									{
										go1.currentBoundingBox.LeanBelow(tileBoundingBox, (uint)Configuration.Tile.Subpx.Height);
										go1.FixVerticalVelocity();
										go1.PushedDown = true;

										oraFoundWall = true;
										foundCollision = true;
									}
								}

							}
							if (oraFoundSpikes && !oraFoundWall)
							{
								go1.InteractionFlags |= (uint)(Ora.FlagTypes.Hurt);
							}
						}
					}

					//if (o == 0)
					//{ // ORA
					//	GameObject linked = _objectPool[o + 1]; // SCYTHE
					//	linked.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.linkedPosition;
					//}

					go1.linkedObject?.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.linkedPosition;
					go1.secondLinkedObject?.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.secondLinkedPosition;
				}


				for (int u = o + 1; u < _objectPool.Length; u++)
				{
					GameObject go2 = _objectPool[u];
					if (go2.Type == GameObject.Types.NONEXISTENT || go2.Type == GameObject.Types.RESERVED || go2.Type == GameObject.Types.SPAWNING) continue;

					GameObject.CheckVerticalCollisions(go1, go2);
				}

			}
		}

        private void SeparatePushables()
        {
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject go1 = _objectPool[o];





				if (go1.Type != GameObject.Types.PUSHABLE) continue;

				for (int u = o + 1; u < _objectPool.Length; u++)
				{
					GameObject go2 = _objectPool[u];
					if (go2.Type != GameObject.Types.PUSHABLE) continue;

					if (go1.currentBoundingBox & go2.currentBoundingBox)
					{
						//if (

						//	!go1.PushedRight && !go1.PushedLeft &&
						//	!go2.PushedRight && !go2.PushedLeft &&
						//	!go1.PushedPreviouslyRight && !go1.PushedPreviouslyLeft &&
						//	!go2.PushedPreviouslyRight && !go2.PushedPreviouslyLeft &&
						//	!go1.PushedDown && !go1.PushedUp &&
						//	!go2.PushedDown && !go2.PushedUp &&
						//	!go1.PushedPreviouslyDown && !go1.PushedPreviouslyUp &&
						//	!go2.PushedPreviouslyDown && !go2.PushedPreviouslyUp
						//	)
						{
							//GameObject.HorizontalSeparation(go1, go2);
							//GameObject.VerticalSeparation(go1, go2);
							GameObject.Interaction(go1, go2);
							GameObject.Separation(go1, go2);
						}

					}
				}
				//if (o == 0)
				//{ // ORA
				//	GameObject linked = _objectPool[o + 1]; // SCYTHE
				//	linked.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.linkedPosition;
				//}
				go1.linkedObject?.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.linkedPosition;
				go1.secondLinkedObject?.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.secondLinkedPosition;
			}
		}

		private void SeparatePushablesFromImmobile()
		{
			for (int o = 0; o < _objectPool.Length; o++)
			{
				GameObject go1 = _objectPool[o];

				if (go1.Type != GameObject.Types.PUSHABLE) continue;

				for (int u = 0; u < _objectPool.Length; u++)
				{
					if (o == u) continue;
					GameObject go2 = _objectPool[u];

					if(go2.Type == GameObject.Types.FLUID && go1.currentBoundingBox & go2.currentBoundingBox)
					{
						go1.VelocityAround = go2.FluidVelocity;
						go1.FrictionCoefficientAround = go2.FluidCoefficient;
						go1.Gravity = go2.FluidGravity;
					}

					if (go2.Type != GameObject.Types.IMMOBILE && go2.Type != GameObject.Types.UNSTOPPABLE) continue;

					if (go1.currentBoundingBox & go2.currentBoundingBox)
					{
						if (
							true
							//!go1.PushedRight && !go1.PushedLeft &&
							//!go1.PushedPreviouslyRight && !go1.PushedPreviouslyLeft &&
							//!go1.PushedDown && !go1.PushedUp &&
							//!go1.PushedPreviouslyDown && !go1.PushedPreviouslyUp
							)
						{
							GameObject.Interaction(go1, go2);
							go1.SeparationFrom(go2.currentBoundingBox);
						}

					}
				}

				TileSpan tileSpan = go1.currentBoundingBox.GetTileSpan();
				for (int row = (int)tileSpan.Top;
					row <= (int)tileSpan.Bottom;
					row++)
				{
					for (int col = (int)tileSpan.Left;
						col <= (int)tileSpan.Right;
						col++)
					{
						Tile tile = _level.GetTile(col, row);
						if (!tile.IsSolid()) continue;
						ObjectBoundingBox tileBoundingBox = ObjectBoundingBox.FromTile((uint)col, (uint)row);
						if (go1.currentBoundingBox & tileBoundingBox)
						{
							go1.SeparationFrom(tileBoundingBox);
						}
					}
				}

				//if (o == 0)
				//{ // ORA
				//	GameObject linked = _objectPool[o + 1]; // SCYTHE
				//	linked.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.linkedPosition;
				//}
				go1.linkedObject?.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.linkedPosition;
				go1.secondLinkedObject?.currentBoundingBox.Position = go1.currentBoundingBox.Position + go1.secondLinkedPosition;

			}

		}

		public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            _animationFrame++;

			Render(spriteBatch);

        }
        public override void Exit()
        {
            GameObject.inputHandler = null;
			GameObject.globalAssets = null;
			GameObject.signalFlags = null;

			switch (_payload.Configuration)
			{
				case GameplayPayload.GameplayConfiguration.NORMAL_GAMEPLAY:
					break;
				case GameplayPayload.GameplayConfiguration.RECORD_AUTOPLAY:
					_inputHandler.StopRecording();
					break;
				case GameplayPayload.GameplayConfiguration.PLAY_AUTOPLAY:
					_inputHandler.StopPlayback();
					break;
			}

			// if allocating on Enter, dispose here
			base.Exit();
			MediaPlayer.Stop();
        }

		private void Render(SpriteBatch spriteBatch)
        {
			if(_level == null) return;
			IBackground background = CurrentBackground switch
			{
				0x11 => Background2.Instance,
				0x12 => Background3.Instance,
				_ => Background1.Instance,
			};
			Renderer.RenderBackground(
				spriteBatch,
				CameraPosition,
				_assets.Atlas!,
				background
			);
			

			Renderer.RenderTiles(
				spriteBatch,
				_level,
				CameraPosition,
				_animationFrame,
				_assets.StaticSpritesheet!,
				_assets.AnimatedSpritesheet!,
				_assets.Atlas!
				);
			Renderer.RenderObjects(
				spriteBatch,
				_objectPool,
				CameraPosition,
				_assets.Atlas!
				);
			if(ApplyDarkness)
			{
				Renderer.RenderDarkness(
					spriteBatch,
					GetPlayerPosition(),
					IsPlayerEmittingLight(),
					CameraPosition,
					_assets.Atlas!,
					DarknessFrom,
					DarknessTo
					);

			}
	
			if(_payload.Configuration == GameplayPayload.GameplayConfiguration.RECORD_AUTOPLAY)
			{
				spriteBatch.Draw(
					_assets.Atlas,
					new Vector2(),
					new Rectangle(608, 784, 32, 16),
					Color.White
					);
				if((_animationFrame & 0x10) == 0x10)
				{
					spriteBatch.Draw(
						_assets.Atlas,
						new Vector2(32,0),
						new Rectangle(640, 784, 16, 16),
						Color.White
						);
				}
			}

			if(state == State.WAITING_ABOVE)
			{
				PxPosition mermaidSprite = ((cutsceneTimer >> 3) & 0x03) switch
				{
					0 => new PxPosition(256 + 32, 512),
					2 => new PxPosition(256 + 64, 512),
					_ => new PxPosition(256, 512),
				};
				spriteBatch.Draw(
					_assets.Atlas,
					new Vector2(Configuration.VisibleScreen.Px.Width/2 + (cutsceneTimer - 45) * 4 ,32),
					new Rectangle((int)mermaidSprite.X, (int)mermaidSprite.Y, 32, 32),
					Color.White,
					0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f
					);
			}
			if (state == State.LEVEL1_ENDING)
			{
				spriteBatch.Draw(
					_assets.Atlas,
					new Vector2(),
					new Rectangle(384, 512, 192, 128),
					Color.White
				);
			}
			_cutsceneManager.Draw(spriteBatch, _assets.Atlas!);
			
			if(state == State.PAUSED || state == State.RESUMING || state == State.GOING_TO_CHECKPOINT || state == State.RESTARTING || state == State.EXITING 
				)
			{
				SpriteEffects transparencyEffect = SpriteEffects.None;
				if((_animationFrame & 1) == 1)
				{
					transparencyEffect = SpriteEffects.FlipHorizontally;
				}
				for(int j = 0; j < 8; j++)
				{
					for(int i = 0; i < 12; i++)
					{
						spriteBatch.Draw(
							_assets.Atlas,
							new Vector2(i*16, j*16),
							new Rectangle(416, 1008, 16, 16),
							Color.White,
							0f, Vector2.Zero, 1f, transparencyEffect, 0f
							);
					}
				}

				spriteBatch.Draw(
					_assets.Atlas,
					new Vector2((pauseFramePosition>>1) - 16*3 - (enteringPauseFramePosition>>2), -(pauseFramePosition>>1)),
					new Rectangle(528, 384, 128, 128),
					Color.White);

				spriteBatch.Draw(
					_assets.Atlas,
					new Vector2( -(pauseFramePosition >> 1) + 16 * 7 + (enteringPauseFramePosition>>2), (pauseFramePosition >> 1)),
					new Rectangle(688, 384, 128, 128),
					Color.White);


				spriteBatch.Draw(
					_assets.Atlas,
					new Vector2(32 - (enteringPauseOptionsPosition>>2), 32),
					new Rectangle(832, 416, 160, 64),
					Color.White);



				spriteBatch.Draw(
					_assets.Atlas,
					new Vector2(16 - (enteringPauseOptionsPosition>>2), 32 + 16 * (int)selectedPauseOption),
					new Rectangle(816, 416, 16, 16),
					Color.White
				);


			}

			if(state == State.ENTERING_GAMEPLAY || state == State.ENTERING_BOSS_STAGE || state == State.LEVEL1_ENDING)
			{
				LoadingEffectEnd.Instance.Draw(spriteBatch, _assets.Atlas!);
			}
			else if(state == State.GOING_TO_CHECKPOINT || state == State.RESTARTING || state == State.EXITING || state == State.GOING_TO_BOSS_STAGE ||
				(state == State.MERMAID_DEFEATED && cutsceneTimer > 90))
			{
				LoadingEffectStart.Instance.Draw(spriteBatch, _assets.Atlas!);
			}
			

		}
    }
}
