using UnityEngine;
using System.Collections;

public class TempTestingCancel : MonoBehaviour {
    public bool isTweening = false;
    public bool tweenOverride = false;
    private LTDescr tween;
 
    // Use this for initialization
    void Start () {
        tween = LeanTween.move(gameObject, transform.position + Vector3.one*3f, Random.Range(2,2) ).setRepeat(-1).setLoopClamp ();
    }
 
    public void Update () {
        if(tween != null){
            isTweening = LeanTween.isTweening(gameObject);
            if(tweenOverride){
             
                // this next line works  
                //tween.cancel();
 
                // this one doesn't
                LeanTween.cancel(gameObject);
            }
        }
    }
}

public class TestingEverything : MonoBehaviour {

	public GameObject cube1;
	public GameObject cube2;


	private bool eventGameObjectWasCalled = false, eventGeneralWasCalled = false;
	private LTDescr lt1;
	private LTDescr lt2;
	private LTDescr lt3;
	private LTDescr lt4;
	private LTDescr[] groupTweens;
	private GameObject[] groupGOs;
	private int groupTweensCnt;

	void Start () {
		LeanTest.expected = 10;

		// add a listener
		LeanTween.addListener(cube1, 0, eventGameObjectCalled);

		// dispatch event that is received
		LeanTween.dispatchEvent(0);
		LeanTest.debug("EVENT GAMEOBJECT RECEIVED", eventGameObjectWasCalled );

		// do not remove listener
		LeanTest.debug("EVENT GAMEOBJECT NOT REMOVED", LeanTween.removeListener(cube2, 0, eventGameObjectCalled)==false );
		// remove listener
		LeanTest.debug("EVENT GAMEOBJECT REMOVED", LeanTween.removeListener(cube1, 0, eventGameObjectCalled) );

		// add a listener
		LeanTween.addListener(1, eventGeneralCalled);
		
		// dispatch event that is received
		LeanTween.dispatchEvent(1);
		LeanTest.debug("EVENT ALL RECEIVED", eventGeneralWasCalled );

		// remove listener
		LeanTest.debug("EVENT ALL REMOVED", LeanTween.removeListener( 1, eventGeneralCalled) );

		lt1 = LeanTween.move( cube1, new Vector3(3f,2f,0.5f), 1.1f );
		LeanTween.move( cube2, new Vector3(-3f,-2f,-0.5f), 1.1f );

		StartCoroutine( timeBasedTesting() );
	}

	IEnumerator timeBasedTesting(){
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		// Groups of tweens testing
		groupTweens = new LTDescr[ 300 ];
		groupGOs = new GameObject[ groupTweens.Length ];
		groupTweensCnt = 0;
		for(int i = 0; i < groupTweens.Length; i++){
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Destroy( cube.GetComponent( typeof(BoxCollider) ) as Component );
			cube.transform.position = new Vector3(0,0,i*3);
			cube.name = "c"+i;
			groupGOs[i] = cube;
			groupTweens[i] = LeanTween.move(cube, transform.position + Vector3.one*3f, 0.2f ).setOnComplete(groupTweenFinished);
		}
		LeanTween.delayedCall(0.42f, groupTweensFinished);

		yield return new WaitForEndOfFrame();

		lt1.cancel();
		LeanTween.cancel(cube2);

		int tweenCount = 0;
		for(int i = 0; i < groupTweens.Length; i++){
			if(LeanTween.isTweening( groupGOs[i] ))
				tweenCount++;
			if(i%3==0)
				LeanTween.pause( groupGOs[i] );
			else if(i%3==1)
				groupTweens[i].pause();
			else
				LeanTween.pause( groupTweens[i].id );
		}
		LeanTest.debug("GROUP ISTWEENING", tweenCount==groupTweens.Length );

		yield return new WaitForEndOfFrame();

		tweenCount = 0;
		for(int i = 0; i < groupTweens.Length; i++){
			if(i%3==0)
				LeanTween.resume( groupGOs[i] );
			else if(i%3==1)
				groupTweens[i].resume();
			else
				LeanTween.resume( groupTweens[i].id );

			if(i%2==0 ? LeanTween.isTweening( groupTweens[i].id ) : LeanTween.isTweening( groupGOs[i] ) )
				tweenCount++;
		}
		LeanTest.debug("GROUP RESUME", tweenCount==groupTweens.Length );

		LeanTest.debug("CANCEL TWEEN LTDESCR", LeanTween.isTweening(cube1)==false );
		LeanTest.debug("CANCEL TWEEN LEANTWEEN", LeanTween.isTweening(cube2)==false );
	}

	void groupTweenFinished(){
		groupTweensCnt++;
	}

	void groupTweensFinished(){
		LeanTest.debug("GROUP FINISH", groupTweensCnt==groupTweens.Length);
	}

	void eventGameObjectCalled( LTEvent e ){
		eventGameObjectWasCalled = true;
	}

	void eventGeneralCalled( LTEvent e ){
		eventGeneralWasCalled = true;
	}

}