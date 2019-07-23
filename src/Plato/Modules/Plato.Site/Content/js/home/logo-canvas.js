$(function (win, doc, $) {

    "use strict";
    
    $('[data-provide="logo"]').each(function () {
        init($(this)[0]);
    });

    function init(elCanvas) {

        var engine = new win.BABYLON.Engine(elCanvas, true),
            lightPos = new win.BABYLON.Vector3(0, 8, 0),
            lightDiffuse = new win.BABYLON.Color3(1, 1, 1);

        // scene
        var scene = (() => {

            // Scene
            var scene = new win.BABYLON.Scene(engine);
            scene.clearColor = new win.BABYLON.Color4(0, 0, 0, 0);

            // Materials

            var matEmit = new win.BABYLON.StandardMaterial('matEmit', scene);
            matEmit.emissiveColor = lightDiffuse;
            matEmit.disableLighting = true;

            var matMetal = new win.BABYLON.PBRMetallicRoughnessMaterial('matMetal', scene);
            matMetal.metallic = 0.9;
            matMetal.roughness = 1;
            matMetal.baseColor = new win.BABYLON.Color3(1, 1, 1);

            var matWall = new BABYLON.PBRMetallicRoughnessMaterial('matWall', scene);
            matWall.metallic = 0.1;
            matWall.roughness = 0;
            matWall.baseColor = new win.BABYLON.Color3(1, 1, 1);

            var whiteMaterial = new win.BABYLON.StandardMaterial("redMaterial", scene);
            whiteMaterial.baseColor = new win.BABYLON.Color3(1, 1, 1);
            whiteMaterial.diffuseColor = new win.BABYLON.Color3(0, 0, 0);
            whiteMaterial.specularColor = new win.BABYLON.Color3(0.8, 0.8, 0.8);
            whiteMaterial.emissiveColor = new win.BABYLON.Color3(0.8, 0.8, 0.8);
            whiteMaterial.ambientColor = new win.BABYLON.Color3(0, 0, 0);

            var redMaterial = new win.BABYLON.StandardMaterial("redMaterial", scene);
            redMaterial.diffuseColor = new win.BABYLON.Color3(0, 0, 0);
            redMaterial.specularColor = new win.BABYLON.Color3(1, 0.8, 0.8);
            redMaterial.emissiveColor = new win.BABYLON.Color3(1, 0, 0);
            redMaterial.ambientColor = new win.BABYLON.Color3(1, 0, 0);
            redMaterial.alpha = 0.7;

            var blueMaterial = new win.BABYLON.StandardMaterial("blueMaterial", scene);
            blueMaterial.diffuseColor = new win.BABYLON.Color3(0, 0, 0);
            blueMaterial.specularColor = new win.BABYLON.Color3(1, 1, 1);
            blueMaterial.emissiveColor = new win.BABYLON.Color3(0, 0, 1);
            blueMaterial.ambientColor = new win.BABYLON.Color3(0, 0, 1);
            blueMaterial.alpha = 0.7;

            var greenMaterial = new win.BABYLON.StandardMaterial("greenMaterial", scene);
            greenMaterial.diffuseColor = new win.BABYLON.Color3(0, 0, 0);
            greenMaterial.specularColor = new win.BABYLON.Color3(1, 1, 1);
            greenMaterial.emissiveColor = new win.BABYLON.Color3(0, 1, 0);
            greenMaterial.ambientColor = new win.BABYLON.Color3(0, 1, 0);
            greenMaterial.alpha = 0.7;

            var yellowMaterial = new win.BABYLON.StandardMaterial("yellowMaterial", scene);
            yellowMaterial.diffuseColor = new win.BABYLON.Color3(0, 0, 0);
            yellowMaterial.specularColor = new win.BABYLON.Color3(1, 1, 1);
            yellowMaterial.emissiveColor = new win.BABYLON.Color3(1, 1, 0.35);
            yellowMaterial.ambientColor = new win.BABYLON.Color3(0, 0, 0);
            yellowMaterial.alpha = 0.8;

            var alphaMaterial = new win.BABYLON.StandardMaterial("alphaMaterial", scene);
            alphaMaterial.alpha = 0;

            // meshes
            var torus = win.BABYLON.MeshBuilder.CreateBox('torus', { width: 1, height: 1, depth: 1 }, scene);
            torus.position.y = 0;
            torus.position.x = 0.5;
            torus.sideOrientation = win.BABYLON.Mesh.FRONTSIDE;
            torus.material = alphaMaterial;

            var bulb = win.BABYLON.MeshBuilder.CreateSphere('bulb', { diameter: 4 }, scene);
            bulb.position = lightPos;
            bulb.material = alphaMaterial;

            // Coords
            var matrix = [
                new win.BABYLON.Vector2(0, 4),
                new win.BABYLON.Vector2(0, -4),
                new win.BABYLON.Vector2(1, -3),
                new win.BABYLON.Vector2(1, 3),
            ];

            var polyMesh = new win.BABYLON.PolygonMeshBuilder("polytri2", matrix, scene);

            var yellowBox = polyMesh.build(true, 1);
            yellowBox.position.y = 4;
            yellowBox.position.x = 1;
            yellowBox.rotation.z = Math.PI / -2;
            yellowBox.material = yellowMaterial;

            var greenBox = polyMesh.build(true, 1);
            greenBox.position.y = -4;
            greenBox.rotation.z = Math.PI / 2;
            greenBox.material = greenMaterial;

            var blueBox = polyMesh.build(true, 1);
            blueBox.position.z = -4;
            blueBox.rotation.y = Math.PI / -2;
            blueBox.rotation.x = Math.PI / 2;
            blueBox.material = blueMaterial;

            var redBox = polyMesh.build(true, 1);
            redBox.position.z = 4;
            redBox.position.x = 1;
            redBox.rotation.y = Math.PI / 2;
            redBox.rotation.x = Math.PI / 2;
            redBox.material = redMaterial;

            // Merge lines into a single mesh
            var mesh = win.BABYLON.Mesh.MergeMeshes([yellowBox, greenBox, blueBox, redBox],
                true,
                true,
                undefined,
                false,
                true);
            mesh.rotation.y = 0;
            mesh.rotation.x = Math.PI / 4;
            
            // Particle system
          
            var particleSystem = new win.BABYLON.ParticleSystem("particles", 2000, scene);

            //Texture of each particle
            particleSystem.particleTexture =
                new win.BABYLON.Texture("https://www.babylonjs-playground.com/textures/flare.png", scene);

            // Where the particles come from
            particleSystem.emitter = torus;
            particleSystem.minEmitBox = new win.BABYLON.Vector3(-1, 10, 0); // Starting all from
            particleSystem.maxEmitBox = new win.BABYLON.Vector3(1, 1, 1); // To...

            // Colors of all particles
            particleSystem.color1 = new win.BABYLON.Color4(0.7, 0.8, 1.0, 1.0);
            particleSystem.color2 = new win.BABYLON.Color4(0.2, 0.5, 1.0, 1.0);
            particleSystem.colorDead = new win.BABYLON.Color4(0, 0, 0.2, 0.0);

            // Size of each particle (random between...
            particleSystem.minSize = 0.1;
            particleSystem.maxSize = 0.4;

            // Life time of each particle (random between...
            particleSystem.minLifeTime = 0.3;
            particleSystem.maxLifeTime = 20.5;

            // Emission rate
            particleSystem.emitRate = 50;
            particleSystem.blendMode = win.BABYLON.ParticleSystem.BLENDMODE_ONEONE;

            // Set the gravity of all particles
            particleSystem.gravity = new win.BABYLON.Vector3(0, -9.81, 0);

            // Direction of each particle after it has been emitted
            particleSystem.direction1 = new win.BABYLON.Vector3(-10, 10, 3);
            particleSystem.direction2 = new win.BABYLON.Vector3(10, 10, -3);

            // Angular speed, in radians
            particleSystem.minAngularSpeed = 0;
            particleSystem.maxAngularSpeed = Math.PI;

            // Speed
            particleSystem.minEmitPower = 0.5;
            particleSystem.maxEmitPower = 1;
            particleSystem.updateSpeed = 0.005;

            // Start the particle system
            particleSystem.start();
            
            // Lights

            var light = new win.BABYLON.PointLight('light1', lightPos, scene);
            light.diffuse = lightDiffuse;
            light.intensity = 200;
            light.excludedMeshes.push(bulb);
            mesh.receiveShadows = true;

            // Camera

            var camera =
                new win.BABYLON.ArcRotateCamera('camera', Math.PI / 4, Math.PI * 3 / 8, 15, torus, scene, true);
            camera.upperBetaLimit = Math.PI * 3 / 4;
            camera.wheelPrecision = 10;
            camera.upperRadiusLimit = 15;
            camera.lowerRadiusLimit = 5;

            // Action

            var t = 0.0,
                i = 0.0;

            scene.registerBeforeRender(function() {
                camera.alpha = 4.0 * (Math.PI / 10 + Math.cos(t / 20));
                camera.beta = 2.0 * (Math.PI / 10 + Math.sin(t / 20));
                t += 0.05;
            });
            
            engine.runRenderLoop(() => {
                i += 0.008;
                lightPos.x = -Math.cos(i) * 8;
                lightPos.z = Math.sin(i) * 8;
            });
            return scene;
        })();

        // Loop
        engine.runRenderLoop(() => {
            scene.render();
        });

        // Events
        addEventListener('resize',
            () => {
                engine.resize();
            });

    }

}(window, document, jQuery));