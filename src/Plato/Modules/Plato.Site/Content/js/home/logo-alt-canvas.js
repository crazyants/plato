$(function (win, doc, $) {

    "use strict";
    
    $('[data-provide="logo-alt"]').each(function () {
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
            torus.position.x = 0;
            torus.sideOrientation = win.BABYLON.Mesh.FRONTSIDE;
            torus.material = alphaMaterial;

            var bulb = win.BABYLON.MeshBuilder.CreateSphere('bulb', { diameter: 4 }, scene);
            bulb.position = lightPos;
            bulb.material = alphaMaterial;

            var redBox = win.BABYLON.MeshBuilder.CreateBox('redBox', { width: 3, height: 3, depth: 3 }, scene);
            redBox.position.y = -1.5;
            redBox.position.z = 1.5;
            redBox.material = redMaterial;

            var greenBox = win.BABYLON.MeshBuilder.CreateBox('greenBox', { width: 3, height: 3, depth: 3 }, scene);
            greenBox.position.y = -1.5;
            greenBox.position.z = -1.5;
            greenBox.material = greenMaterial;

            var blueBox = win.BABYLON.MeshBuilder.CreateBox('blueBox', { width: 3, height: 3, depth: 3 }, scene);
            blueBox.position.z = -1.5;
            blueBox.position.y = 1.5;
            blueBox.position.x = 0;
            blueBox.material = blueMaterial;

            var yellowBox = win.BABYLON.MeshBuilder.CreateBox('yellowBox', { width: 3, height: 3, depth: 3 }, scene);
            yellowBox.position.z = 1.5;
            yellowBox.position.x = 0;
            yellowBox.position.y = 1.5;
            yellowBox.material = yellowMaterial;



            // Merge lines into a single mesh
            var mesh = win.BABYLON.Mesh.MergeMeshes([yellowBox, greenBox, blueBox, redBox],
                true,
                true,
                undefined,
                false,
                true);
            mesh.rotation.y = 0;
            mesh.rotation.x = Math.PI / 4;

            

            // Lights

            var light = new win.BABYLON.PointLight('light1', lightPos, scene);
            light.diffuse = lightDiffuse;
            light.intensity = 200;
            light.excludedMeshes.push(bulb);
            mesh.receiveShadows = true;

            // Camera

            var camera = new win.BABYLON.ArcRotateCamera('camera', 0, Math.PI / 4, 12, torus, scene, true);
            camera.upperBetaLimit = Math.PI * 3 / 4;
            camera.wheelPrecision = 10;
            camera.upperRadiusLimit = 40;
            camera.lowerRadiusLimit = 5;
                
            // Action

            var t = 0.0;

            scene.registerBeforeRender(function () {

                mesh.rotation.y = t;
                t += 0.01;
            });

            var i = 0;
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