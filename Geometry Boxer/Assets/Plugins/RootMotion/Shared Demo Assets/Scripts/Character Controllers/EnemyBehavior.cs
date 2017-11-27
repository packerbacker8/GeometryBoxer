//using UnityEngine;
//using System;
//using System.Collections.Generic;
//using UnityEngine.AI;
//
//namespace RootMotion.Demos
//{
//    public class EnemyBehavior
//    {
//        public bool drop;
//
//        private Vector3 _targetDir;
//        private Vector3 _newDir;
//        private AnimatorStateInfo _info;
//        private NavMeshAgent _agent;
//        private Animator _anim;
//
//        private string leftSwingAnimation = "SwingProp";
//        private string rightSwingAnimation = "SwingProp";
//        private string getUpProne = "GetUpProne";
//        private string getUpSupine = "GetUpSupine";
//        private string fall = "Fall";
//        private string onGround = "OnGround";
//
//        private GameObject activePlayer;
//        private GameObject[] playerOptions;
//        private AudioSource source;
//        private SFX_Manager sfxManager;
//        private System.Random rand = new System.Random();
//
//        private int attackIndex;
//        private int swingAnimLayer = 1;
//        private int punchAnimLayer = 0;
//        private int animationControllerIndex = 0;
//        private int characterControllerIndex = 2;
//
//        public EnemyBehavior(Vector3 targetDir, Vector3 newDir, AnimatorStateInfo info, NavMeshAgent agent, Animator anim)
//        {
//            _targetDir = targetDir;
//            _newDir = newDir;
//            _info = info;
//            _agent = agent;
//            _anim = anim;
//        }
//        public void defaultMovement()
//        {
//
//        }
//
//        private void defaultBehavior(Vector3 targetDir, Vector3 newDir, AnimatorStateInfo info)
//        {
//            if (!(!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && _anim.GetBool(onGround)))
//            {
//                if (!_agent.isOnOffMeshLink)
//                {
//                    _agent.nextPosition = transform.position;
//                    _agent.enabled = false;
//                }
//                else
//                {
//                    drop = true;
//                }
//
//            }
//            else if (!_agent.enabled)
//            {
//                drop = false;
//                _agent.enabled = true;
//                _agent.nextPosition = transform.position;
//            }
//
//            if (Vector3.Distance(moveTarget.position, this.transform.position) <= attackRange && !this._anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
//            {
//                //Rand controls chance of random attack sound being played, only while source is not already playing
//                if (rand.Next(0, attackRandomAudio) == 1 && sfxManager.maleAttack.Count > 0 && !source.isPlaying)
//                {
//                    source.PlayOneShot(sfxManager.maleAttack[rand.Next(0, sfxManager.maleAttack.Count)]);
//                }
//
//                //If puppet is down, does not try to attack player during stand up anim
//                if ((!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && !info.IsName(onGround)))
//                {
//                    //This is for when puppet has melee object in hand
//                    if (characterPuppet.propRoot.currentProp != null)
//                    {
//                        _anim.Play(rightSwingAnimation, swingAnimLayer);
//                    }
//                    else//No melee object in hand of puppet
//                    {
//                        _anim.Play(rightSwingAnimation, punchAnimLayer);
//                    }
//                }
//            }
//            if (_agent.enabled)
//            {
//                if (Vector3.Distance(moveTarget.position, transform.position) > stoppingThreshold * stoppingDistance)
//                {
//                    _agent.destination = moveTarget.position;
//                    state.move = _agent.velocity;
//                }
//                else
//                {
//                    _agent.destination = transform.position;
//                    state.move = _agent.velocity;
//                }
//            }
//
//            //Always rotate to face the player
//            transform.rotation = Quaternion.LookRotation(newDir);
//        }
//
//        private void jumpBehavior(Vector3 targetDir, Vector3 newDir, AnimatorStateInfo info)
//        {
//            if (!(!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && _anim.GetBool(onGround)) && !_agent.isOnOffMeshLink)
//            {
//                //_agent.updatePosition = false;
//                //_agent.nextPosition = transform.position;
//                _agent.nextPosition = transform.position;
//                _agent.enabled = false;
//                drop = false;
//
//            }
//            else if (!_agent.enabled)
//            {
//                drop = false;
//                _agent.enabled = true;
//                _agent.nextPosition = transform.position;
//            }
//            //_agent.updatePosition = false; //New line automatically makes it where the _agent no longer affects movement
//            if (Vector3.Distance(moveTarget.position, this.transform.position) <= attackRange && !this._anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
//            {
//
//                //Rand controls chance of random attack sound being played, only while source is not already playing
//                if (rand.Next(0, attackRandomAudio) == 1 && sfxManager.maleAttack.Count > 0 && !source.isPlaying)
//                {
//                    source.PlayOneShot(sfxManager.maleAttack[rand.Next(0, sfxManager.maleAttack.Count)]);
//                }
//
//                //If puppet is down, does not try to attack player during stand up anim
//                if ((!info.IsName(getUpProne) && !info.IsName(getUpSupine) && !info.IsName(fall) && !info.IsName(onGround)))
//                {
//                    //This is for when puppet has melee object in hand
//                    if (characterPuppet.propRoot.currentProp != null)
//                    {
//                        _anim.Play(rightSwingAnimation, swingAnimLayer);
//                    }
//                    else//No melee object in hand of puppet
//                    {
//                        _anim.Play(rightSwingAnimation, punchAnimLayer);
//                    }
//                }
//            }
//            if (_agent.enabled)
//            {
//                if (Vector3.Distance(moveTarget.position, transform.position) < jumpDistance + jumpThreshold && Vector3.Distance(moveTarget.position, transform.position) > jumpDistance - jumpThreshold)
//                {
//
//                    _agent.enabled = false;
//                    state.jump = true;
//                }
//                else if (Vector3.Distance(moveTarget.position, transform.position) > stoppingThreshold * stoppingDistance)
//                {
//                    _agent.destination = moveTarget.position;
//                    state.move = _agent.velocity;
//                }
//                else
//                {
//                    _agent.destination = transform.position;
//                    state.move = _agent.velocity;
//                }
//            }
//
//            //Always rotate to face the player
//            transform.rotation = Quaternion.LookRotation(newDir);
//        }
//    }
//}