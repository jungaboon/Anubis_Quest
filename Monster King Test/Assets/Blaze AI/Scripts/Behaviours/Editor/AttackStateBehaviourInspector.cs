using UnityEditor;
using UnityEngine;

namespace BlazeAISpace
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AttackStateBehaviour))]

    public class AttackStateBehaviourInspector : Editor
    {
        #region SERIALIZED PROPERTIES

        SerializedProperty moveSpeed,
        turnSpeed,

        idleAnim,
        moveAnim,
        idleMoveT,

        equipWeapon,
        equipAnim,
        equipDuration,
        onEquipEvent,

        unEquipAnim,
        unEquipDuration,
        onUnEquipEvent,
        equipAnimT,

        onStateEnter,
        onStateExit,

        ranged,
        distanceFromEnemy,
        attackDistance,
        layersCheckOnAttacking,
        attacks,
        attackEvent,
        chaseOnEvade,
        attackInIntervals,
        attackInIntervalsTime,

        callOthers,
        callRadius,
        showCallRadius,
        agentLayersToCall,
        callPassesColliders,
        callOthersTime,
        receiveCallFromOthers,

        moveBackwards,
        moveBackwardsDistance,
        moveBackwardsSpeed,
        moveBackwardsAnim,
        moveBackwardsAnimT,

        turnToTarget,
        turnSensitivity,
        useTurnAnims,

        strafe,
        strafeDirection,
        strafeSpeed,
        strafeTime,
        strafeWaitTime,
        leftStrafeAnim,
        rightStrafeAnim,
        strafeAnimT,
        strafeLayersToAvoid,

        onAttackRotate,
        onAttackRotateSpeed,

        searchLocationRadius,
        timeToStartSearch,
        searchPoints,
        searchPointAnim,
        pointWaitTime,
        endSearchAnim,
        endSearchAnimTime,
        searchAnimsT,
        playAudioOnSearchStart,
        playAudioOnSearchEnd,

        returnPatrolAnim,
        returnPatrolAnimT,
        returnPatrolTime,
        playAudioOnReturnPatrol,

        playAttackIdleAudio,
        attackIdleAudioTime,

        playAudioOnChase,
        alwaysPlayOnChase,

        playAudioOnMoveToAttack,
        alwaysPlayOnMoveToAttack;

        #endregion

        #region EDITOR VARIABLES

        bool displayAttackEvents = true;
        int spaceBetween = 20;

        string[] tabs = {"General", "Attack", "Attack-Idle", "Call Others", "Search & Return", "Audios & Events"};
        int tabSelected = 0;
        int tabIndex = -1;

        #endregion

        #region EDITOR METHODS

        void OnEnable()
        {
            GetSelectedTab(); 


            moveSpeed = serializedObject.FindProperty("moveSpeed");
            turnSpeed = serializedObject.FindProperty("turnSpeed");

            idleAnim = serializedObject.FindProperty("idleAnim");
            moveAnim = serializedObject.FindProperty("moveAnim");
            idleMoveT = serializedObject.FindProperty("idleMoveT");

            equipWeapon = serializedObject.FindProperty("equipWeapon");
            equipAnim = serializedObject.FindProperty("equipAnim");
            equipDuration = serializedObject.FindProperty("equipDuration");
            equipAnimT = serializedObject.FindProperty("equipAnimT");
            onEquipEvent = serializedObject.FindProperty("onEquipEvent");
    
            unEquipAnim = serializedObject.FindProperty("unEquipAnim");
            unEquipDuration = serializedObject.FindProperty("unEquipDuration");
            onUnEquipEvent = serializedObject.FindProperty("onUnEquipEvent");

            onStateEnter = serializedObject.FindProperty("onStateEnter");
            onStateExit = serializedObject.FindProperty("onStateExit");

            ranged = serializedObject.FindProperty("ranged");
            distanceFromEnemy = serializedObject.FindProperty("distanceFromEnemy");
            attackDistance = serializedObject.FindProperty("attackDistance");
            layersCheckOnAttacking = serializedObject.FindProperty("layersCheckOnAttacking");
            attacks = serializedObject.FindProperty("attacks");
            attackEvent = serializedObject.FindProperty("attackEvent");
            chaseOnEvade = serializedObject.FindProperty("chaseOnEvade");

            attackInIntervals = serializedObject.FindProperty("attackInIntervals");
            attackInIntervalsTime = serializedObject.FindProperty("attackInIntervalsTime");

            callOthers = serializedObject.FindProperty("callOthers");
            callRadius = serializedObject.FindProperty("callRadius");
            showCallRadius = serializedObject.FindProperty("showCallRadius");
            agentLayersToCall = serializedObject.FindProperty("agentLayersToCall");
            callPassesColliders = serializedObject.FindProperty("callPassesColliders");
            callOthersTime = serializedObject.FindProperty("callOthersTime");
            receiveCallFromOthers = serializedObject.FindProperty("receiveCallFromOthers");

            moveBackwards = serializedObject.FindProperty("moveBackwards");
            moveBackwardsDistance = serializedObject.FindProperty("moveBackwardsDistance");
            moveBackwardsSpeed = serializedObject.FindProperty("moveBackwardsSpeed");
            moveBackwardsAnim = serializedObject.FindProperty("moveBackwardsAnim");
            moveBackwardsAnimT = serializedObject.FindProperty("moveBackwardsAnimT");

            turnToTarget = serializedObject.FindProperty("turnToTarget");
            turnSensitivity = serializedObject.FindProperty("turnSensitivity");
            useTurnAnims = serializedObject.FindProperty("useTurnAnims");

            strafe = serializedObject.FindProperty("strafe");
            strafeDirection = serializedObject.FindProperty("strafeDirection");
            strafeSpeed = serializedObject.FindProperty("strafeSpeed");
            strafeTime = serializedObject.FindProperty("strafeTime");
            strafeWaitTime = serializedObject.FindProperty("strafeWaitTime");
            leftStrafeAnim = serializedObject.FindProperty("leftStrafeAnim");
            rightStrafeAnim = serializedObject.FindProperty("rightStrafeAnim");
            strafeAnimT = serializedObject.FindProperty("strafeAnimT");
            strafeLayersToAvoid = serializedObject.FindProperty("strafeLayersToAvoid");

            onAttackRotate = serializedObject.FindProperty("onAttackRotate");
            onAttackRotateSpeed = serializedObject.FindProperty("onAttackRotateSpeed");

            searchLocationRadius = serializedObject.FindProperty("searchLocationRadius");
            timeToStartSearch = serializedObject.FindProperty("timeToStartSearch");
            searchPoints = serializedObject.FindProperty("searchPoints");
            searchPointAnim = serializedObject.FindProperty("searchPointAnim");
            pointWaitTime = serializedObject.FindProperty("pointWaitTime");
            endSearchAnim = serializedObject.FindProperty("endSearchAnim");
            endSearchAnimTime = serializedObject.FindProperty("endSearchAnimTime");
            searchAnimsT = serializedObject.FindProperty("searchAnimsT");
            playAudioOnSearchStart = serializedObject.FindProperty("playAudioOnSearchStart");
            playAudioOnSearchEnd = serializedObject.FindProperty("playAudioOnSearchEnd");

            returnPatrolAnim = serializedObject.FindProperty("returnPatrolAnim");
            returnPatrolAnimT = serializedObject.FindProperty("returnPatrolAnimT");
            returnPatrolTime = serializedObject.FindProperty("returnPatrolTime");
            playAudioOnReturnPatrol = serializedObject.FindProperty("playAudioOnReturnPatrol");

            playAttackIdleAudio = serializedObject.FindProperty("playAttackIdleAudio");
            attackIdleAudioTime = serializedObject.FindProperty("attackIdleAudioTime");

            playAudioOnChase = serializedObject.FindProperty("playAudioOnChase");
            alwaysPlayOnChase = serializedObject.FindProperty("alwaysPlayOnChase");

            playAudioOnMoveToAttack = serializedObject.FindProperty("playAudioOnMoveToAttack");
            alwaysPlayOnMoveToAttack = serializedObject.FindProperty("alwaysPlayOnMoveToAttack");
        }

        public override void OnInspectorGUI () 
        {   
            DrawToolbar();
            EditorGUILayout.LabelField("Hover on any property below for insights", EditorStyles.helpBox);
            EditorGUILayout.Space(10);
            
            AttackStateBehaviour script = (AttackStateBehaviour) target;
            tabIndex = -1;

            switch (tabSelected)
            {
                case 0:
                    DrawGeneralTab(script);
                    break;
                case 1:
                    DrawAttackTab(script);
                    break;
                case 2:
                    DrawAttackIdleTab(script);
                    break;
                case 3:
                    DrawCallOthersTab(script);
                    break;
                case 4:
                    DrawSearchAndReturnTab(script);
                    break;
                case 5:
                    DrawAudiosAndEventsTab(script);
                    break;
            }

            EditorPrefs.SetInt("BlazeAttackTabSelected", tabSelected);
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region DRAWING    

        void DrawToolbar()
        {   
            GUILayout.BeginHorizontal();
            
            foreach (var item in tabs) {
                tabIndex++;

                if (tabIndex == 3) {
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space(0.2f);
                    GUILayout.BeginHorizontal();
                }

                if (tabIndex == tabSelected) {
                    // selected button
                    GUILayout.Button(item, BlazeAIEditor.ToolbarStyling(true), GUILayout.MinWidth(105), GUILayout.Height(40));
                }
                else {
                    // unselected buttons
                    if (GUILayout.Button(item, BlazeAIEditor.ToolbarStyling(false), GUILayout.MinWidth(105), GUILayout.Height(40))) {
                        // this will trigger when a button is pressed
                        tabSelected = tabIndex;
                    }
                }
            }

            GUILayout.EndHorizontal();
        }

        void DrawGeneralTab(AttackStateBehaviour script)
        {
            EditorGUILayout.LabelField("Speeds", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(moveSpeed);
            EditorGUILayout.PropertyField(turnSpeed);
            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(idleAnim);
            EditorGUILayout.PropertyField(moveAnim);
            EditorGUILayout.PropertyField(idleMoveT);
            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Equip Weapon", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(equipWeapon);
            
            if (script.equipWeapon) 
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(equipAnim);
                EditorGUILayout.PropertyField(equipDuration);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(equipAnimT);
                EditorGUILayout.Space(spaceBetween);
                
                EditorGUILayout.PropertyField(onEquipEvent);
                EditorGUILayout.Space(5);

                EditorGUILayout.LabelField("Unequip weapon", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(unEquipAnim);
                EditorGUILayout.PropertyField(unEquipDuration);
                EditorGUILayout.Space(spaceBetween);

                EditorGUILayout.PropertyField(onUnEquipEvent);
            }
        }

        void DrawAttackTab(AttackStateBehaviour script)
        {
            EditorGUILayout.LabelField("Attacking & Distances", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(ranged);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(distanceFromEnemy);
            EditorGUILayout.PropertyField(attackDistance);
            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("Friendly-Fire Layers", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(layersCheckOnAttacking);
            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("Add Attacks", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(attacks);
            EditorGUILayout.Space(20);


            displayAttackEvents = EditorGUILayout.Toggle("Display Attack Events", displayAttackEvents);
            if (displayAttackEvents) {
                EditorGUILayout.PropertyField(attackEvent);
                EditorGUILayout.Space();
            }
            else {
                EditorGUILayout.Space(spaceBetween);
            }


            EditorGUILayout.LabelField("Chase Again On Target Evade Attack", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(chaseOnEvade);
            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("Interval Attacks", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(attackInIntervals);
            if (script.attackInIntervals) {
                EditorGUILayout.PropertyField(attackInIntervalsTime);
            }
            EditorGUILayout.Space(spaceBetween);


            EditorGUILayout.LabelField("Attack Rotate", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(onAttackRotateSpeed);
            EditorGUILayout.PropertyField(onAttackRotate);
        }

        void DrawAttackIdleTab(AttackStateBehaviour script)
        {
            EditorGUILayout.LabelField("Move Backwards", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(moveBackwards);
            if (script.moveBackwards) {
                EditorGUILayout.PropertyField(moveBackwardsDistance);
                EditorGUILayout.PropertyField(moveBackwardsSpeed);
                EditorGUILayout.Space();
                
                EditorGUILayout.PropertyField(moveBackwardsAnim);
                EditorGUILayout.PropertyField(moveBackwardsAnimT);
            }


            EditorGUILayout.Space(spaceBetween);
            EditorGUILayout.LabelField("Turning To Target", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(turnToTarget);
            if (script.turnToTarget) {
                EditorGUILayout.PropertyField(turnSensitivity);
                EditorGUILayout.PropertyField(useTurnAnims);
            }


            EditorGUILayout.Space(spaceBetween);
            EditorGUILayout.LabelField("Strafing", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(strafe);
            if (script.strafe) {
                EditorGUILayout.PropertyField(strafeDirection);
                EditorGUILayout.PropertyField(strafeSpeed);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(strafeTime);
                EditorGUILayout.PropertyField(strafeWaitTime);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(leftStrafeAnim);
                EditorGUILayout.PropertyField(rightStrafeAnim);
                EditorGUILayout.PropertyField(strafeAnimT);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(strafeLayersToAvoid);
            }
        }

        void DrawCallOthersTab(AttackStateBehaviour script)
        {
            EditorGUILayout.LabelField("Call Others", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(callOthers);
            if (script.callOthers) {
                EditorGUILayout.Space();
                
                EditorGUILayout.PropertyField(callRadius);
                EditorGUILayout.PropertyField(showCallRadius);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(agentLayersToCall);
                EditorGUILayout.PropertyField(callPassesColliders);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(callOthersTime);
                EditorGUILayout.PropertyField(receiveCallFromOthers);
            }
        }

        void DrawSearchAndReturnTab(AttackStateBehaviour script)
        {
            EditorGUILayout.LabelField("Searching Location", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(searchLocationRadius);
            if (script.searchLocationRadius) {
                EditorGUILayout.PropertyField(timeToStartSearch);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(searchPoints);
                EditorGUILayout.PropertyField(searchPointAnim);
                EditorGUILayout.PropertyField(pointWaitTime);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(endSearchAnim);
                EditorGUILayout.PropertyField(endSearchAnimTime);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(searchAnimsT);
                EditorGUILayout.Space();
                
                EditorGUILayout.PropertyField(playAudioOnSearchStart);
                EditorGUILayout.PropertyField(playAudioOnSearchEnd);

                return;
            }


            EditorGUILayout.Space(spaceBetween);
            EditorGUILayout.LabelField("Returning To Patrol (Alert State)", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(returnPatrolAnim);
            EditorGUILayout.PropertyField(returnPatrolAnimT);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(returnPatrolTime);
            EditorGUILayout.PropertyField(playAudioOnReturnPatrol);
        }

        void DrawAudiosAndEventsTab(AttackStateBehaviour script)
        {   
            EditorGUILayout.LabelField("Audios", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playAttackIdleAudio);
            if (script.playAttackIdleAudio) {
                EditorGUILayout.PropertyField(attackIdleAudioTime);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(playAudioOnChase);
            if (script.playAudioOnChase) {
                EditorGUILayout.PropertyField(alwaysPlayOnChase);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(playAudioOnMoveToAttack);
            if (script.playAudioOnMoveToAttack) {
                EditorGUILayout.PropertyField(alwaysPlayOnMoveToAttack);
            }

            EditorGUILayout.Space(spaceBetween);

            EditorGUILayout.LabelField("State Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(onStateEnter);
            EditorGUILayout.PropertyField(onStateExit);
        }

        void GetSelectedTab()
        {
            if (EditorPrefs.HasKey("BlazeAttackTabSelected")) {
                tabSelected = EditorPrefs.GetInt("BlazeAttackTabSelected");
            }
            else{
                tabSelected = 0;
            }   
        }

        #endregion
    }
}