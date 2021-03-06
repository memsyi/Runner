using UnityEngine;
using System.Collections;

public class PlayerHit : MonoBehaviour
{
	private PlayerSize playerSize;
	private PlayerShoot playerShoot;
	private PlayerForceField playerForceField;
	private PlayerColors playerColors;
	private PlayerExplosion playerExplosion;
	private ForceField forceField;
	private PlayerSounds playerSounds;
//	private PlayerNitro playerNitro;
	
	private int missilesCollided, capsules;
	
	// Use this for initialization
	void Awake()
	{
		forceField = gameObject.GetComponentInChildren<ForceField>();
		
		playerSize 			= GetComponent<PlayerSize>();
		playerShoot			= GetComponent<PlayerShoot>();
		playerForceField	= GetComponent<PlayerForceField>();
		playerColors		= GetComponent<PlayerColors>();
		playerExplosion		= GetComponent<PlayerExplosion>();
		playerSounds		= GetComponent<PlayerSounds>();
//		playerNitro			= GetComponent<PlayerNitro>();
		
		// listen to game events
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
	}
	
	private void OnTriggerEnter ( Collider col ) {
		
		switch( col.gameObject.tag ) 
		{
			case "Coin":
				addCoin();
				col.GetComponent<AudioSource>().Play();
				col.GetComponent<Coin>().Disable();
			break;
			case "Cannon":
				col.GetComponent<CannonPicker>().Fill();
			break;
			case "Pickup":
				
				capsules++;
				col.GetComponent<PickupSound>().Play();
				col.GetComponent<PickupItem>().Picked();
			
				string pickType = col.renderer.material.name.Split('_')[1];
					Picked( pickType );
			
			break;
			case "Missile":
			
				col.gameObject.SetActive(false);
			
				if( !forceField.isOn )
				{
					missilesCollided++;
				
					PlayerDataManager.SetValue( SessionData.MISSILES_COLLIDED, missilesCollided );
					
					if( PlayerDataManager.GetLoadedData().total_missiles_collided >= 10 )
						GooglePlusSocial.SubmitAchievement( GooglePlusSocial.ACHIEVEMENT_PINATA );
					
					playerExplosion.Explode();
				}
				else
				{
					playerSounds.PlaySounds( playerSounds.missileOff );
				
					playerShoot.StopShooting();
					forceField.Hide();
				}
			break;
		} 
	}
	
	void GameStart() {
		capsules = 0;
		missilesCollided = 0;
	}
	
	void GameOver() {
		PlayerDataManager.SetValue( SessionData.CAPSULES, capsules );
	}
	
	void addCoin() {
		
		playerSize.Grow();
		
		GUIManager.AddCoin();
	}
	
	void Picked( string type ) {
		
		switch( type )
		{
			case "Red" : 	playerColors.ChangeColor( PlayerColors.RED ); 	break;
			case "Green" :  playerColors.ChangeColor( PlayerColors.GREEN ); break;
			case "Blue" :  	playerColors.ChangeColor( PlayerColors.BLUE ); 	break;
			case "Yellow" : playerColors.ChangeColor( PlayerColors.YELLOW );break;
//			case "Big" :  	playerSize.ChangeSize( PlayerSize.BIG ); 		break;
//			case "Small" :  playerSize.ChangeSize( PlayerSize.SMALL ); 		break;
			case "Shoot" :  playerShoot.StartShooting(); 					break;
			case "Force" :  playerForceField.Show(); 						break;	
//			case "Nitro" :  playerNitro.Explode(); 							break;	
		}
	}
}

