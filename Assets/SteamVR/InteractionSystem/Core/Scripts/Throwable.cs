//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Basic throwable object
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]
	[RequireComponent( typeof( Rigidbody ) )]
    [RequireComponent( typeof(VelocityEstimator))]
	public class Throwable : MonoBehaviour
	{
		[EnumFlags]
		[Tooltip( "The flags used to attach this object to the hand." )]
		public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.TurnOnKinematic;

        [Tooltip("The local point which acts as a positional and rotational offset to use while held")]
        public Transform attachmentOffset;

		[Tooltip( "How fast must this object be moving to attach due to a trigger hold instead of a trigger press? (-1 to disable)" )]
        public float catchingSpeedThreshold = -1;

        public ReleaseStyle releaseVelocityStyle = ReleaseStyle.GetFromHand;

        [Tooltip("The time offset used when releasing the object with the RawFromHand option")]
        public float releaseVelocityTimeOffset = -0.011f;

        public float scaleReleaseVelocity = 1.1f;

		[Tooltip( "When detaching the object, should it return to its original parent?" )]
		public bool restoreOriginalParent = false;

        

		protected VelocityEstimator velocityEstimator;
        protected bool attached = false;
        protected float attachTime;
        protected Vector3 attachPosition;
        protected Quaternion attachRotation;
        protected Transform attachEaseInTransform;

		public UnityEvent onPickUp;
        public UnityEvent onDetachFromHand;
        public UnityEvent<Hand> onHeldUpdate;

        
        protected RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;

        protected new Rigidbody rigidbody;

        [HideInInspector]
        public Interactable interactable;

        //CreatingBall
        public Transform BallCreatorPosition;
        public GameObject BallCreatorObject;
       // public bool isBallReleased = true;
      //  public bool isBallDestroyed = true;
        //-------------------------------------------------
        protected virtual void Awake()
		{
			velocityEstimator = GetComponent<VelocityEstimator>();
            interactable = GetComponent<Interactable>();



            rigidbody = GetComponent<Rigidbody>();
            rigidbody.maxAngularVelocity = 50.0f;


            if(attachmentOffset != null)
            {
                // remove?
                //interactable.handFollowTransform = attachmentOffset;
            }


           // BallCreatorPosition.position = new Vector3(0.816f, 1.038f, -3.697f);
           // BallCreatorPosition.localScale = new Vector3(3, 3, 3);
        }


        //-------------------------------------------------
        protected virtual void OnHandHoverBegin( Hand hand )
		{
			bool showHint = false;

            // "Catch" the throwable by holding down the interaction button instead of pressing it.
            // Only do this if the throwable is moving faster than the prescribed threshold speed,
            // and if it isn't attached to another hand
            if ( !attached && catchingSpeedThreshold != -1)
            {
                float catchingThreshold = catchingSpeedThreshold * SteamVR_Utils.GetLossyScale(Player.instance.trackingOriginTransform);

                GrabTypes bestGrabType = hand.GetBestGrabbingType();

                if ( bestGrabType != GrabTypes.None )
				{
					if (rigidbody.velocity.magnitude >= catchingThreshold)
					{
						hand.AttachObject( gameObject, bestGrabType, attachmentFlags );
						showHint = false;
					}
				}
			}

			if ( showHint )
			{
                hand.ShowGrabHint();
			}
		}

        public bool IsAttached()
        {
            return attached;
        }

        //-------------------------------------------------
        protected virtual void OnHandHoverEnd( Hand hand )
		{
            hand.HideGrabHint();
		}


        //-------------------------------------------------
        protected virtual void HandHoverUpdate( Hand hand )
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();
            
            if (startingGrabType != GrabTypes.None)
            {
				hand.AttachObject( gameObject, startingGrabType, attachmentFlags, attachmentOffset );
                hand.HideGrabHint();
            }
		}

        //-------------------------------------------------
        protected virtual void OnAttachedToHand( Hand hand )
		{
            //Debug.Log("<b>[SteamVR Interaction]</b> Pickup: " + hand.GetGrabStarting().ToString());

            hadInterpolation = this.rigidbody.interpolation;

            attached = true;

            //Instantiate(BallCreatorObject, BallCreatorPosition.position, BallCreatorPosition.rotation);

            onPickUp.Invoke();

			hand.HoverLock( null );
            
            rigidbody.interpolation = RigidbodyInterpolation.None;
            
		    velocityEstimator.BeginEstimatingVelocity();

			attachTime = Time.time;
			attachPosition = transform.position;
			attachRotation = transform.rotation;

		}


        //-------------------------------------------------
        protected virtual void OnDetachedFromHand(Hand hand)
        {
            attached = false;

            //Debug.Log("at OnDetachedFromHand");
            //Instantiate(BallCreatorObject, BallCreatorPosition.position, BallCreatorPosition.rotation);


            onDetachFromHand.Invoke();

            hand.HoverUnlock(null);
            
            rigidbody.interpolation = hadInterpolation;

            Vector3 velocity;
            Vector3 angularVelocity;

            GetReleaseVelocities(hand, out velocity, out angularVelocity);

            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
        }

        //CreatingBall
        public virtual void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
        {
           // Debug.Log("at GetReleaseVelocities");
            Instantiate(BallCreatorObject, BallCreatorPosition.position, BallCreatorPosition.rotation);
            // isBallReleased = true;
            /*   if (isBallDestroyed)
               {
                   Instantiate(BallCreatorObject, BallCreatorPosition.position, BallCreatorPosition.rotation);
                   isBallDestroyed = false;
               }*/


            if (hand.noSteamVRFallbackCamera && releaseVelocityStyle != ReleaseStyle.NoChange)
                releaseVelocityStyle = ReleaseStyle.ShortEstimation; // only type that works with fallback hand is short estimation.

            switch (releaseVelocityStyle)
            {
                case ReleaseStyle.ShortEstimation:
                   // Debug.Log("at ReleaseStyle.ShortEstimation");

                    velocityEstimator.FinishEstimatingVelocity();
                    velocity = velocityEstimator.GetVelocityEstimate();
                    angularVelocity = velocityEstimator.GetAngularVelocityEstimate();
                    break;
                case ReleaseStyle.AdvancedEstimation:
                    //Debug.Log("at ReleaseStyle.AdvancedEstimation");

                    hand.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
                    break;
                case ReleaseStyle.GetFromHand:
                   // Debug.Log("at ReleaseStyle.GetFromHand");

                    velocity = hand.GetTrackedObjectVelocity(releaseVelocityTimeOffset);
                    angularVelocity = hand.GetTrackedObjectAngularVelocity(releaseVelocityTimeOffset);
                    break;
                default:
                case ReleaseStyle.NoChange:
                   // Debug.Log("at ReleaseStyle.NoChange");

                    velocity = rigidbody.velocity;
                    angularVelocity = rigidbody.angularVelocity;
                    break;
            }

            if (releaseVelocityStyle != ReleaseStyle.NoChange)
                velocity *= scaleReleaseVelocity;
        }

        //-------------------------------------------------
        protected virtual void HandAttachedUpdate(Hand hand)
        {
           // Debug.Log("at HandAttachedUpdate");

            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject, restoreOriginalParent);

                // Uncomment to detach ourselves late in the frame.
                // This is so that any vehicles the player is attached to
                // have a chance to finish updating themselves.
                // If we detach now, our position could be behind what it
                // will be at the end of the frame, and the object may appear
                // to teleport behind the hand when the player releases it.
                //StartCoroutine( LateDetach( hand ) );
            }

            if (onHeldUpdate != null)
                onHeldUpdate.Invoke(hand);
        }


        //-------------------------------------------------
        protected virtual IEnumerator LateDetach( Hand hand )
		{
           // Debug.Log("at LateDetach");

            yield return new WaitForEndOfFrame();

			hand.DetachObject( gameObject, restoreOriginalParent );
		}


        //-------------------------------------------------
        protected virtual void OnHandFocusAcquired( Hand hand )
		{
			gameObject.SetActive( true );
			velocityEstimator.BeginEstimatingVelocity();
		}


        //-------------------------------------------------
        protected virtual void OnHandFocusLost( Hand hand )
		{
           // Debug.Log("at OnHandFocusLost");

            gameObject.SetActive( false );
			velocityEstimator.FinishEstimatingVelocity();
		}
	}

    public enum ReleaseStyle
    {
        NoChange,
        GetFromHand,
        ShortEstimation,
        AdvancedEstimation,
    }
}
