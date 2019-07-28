$(function (win, doc, $) {

    "use strict";
    
    $('[data-provide="plato-canvas"]').each(function() {
        init($(this)[0]);
    });

    function getRnd(max) {
        return Math.floor(Math.random() * Math.floor(max)) + 4;
    }
    
    function init(elCanvas) {

        // Default camera position
        var cameraX = Math.PI * 6,
            cameraY = Math.PI * 3 / 8,
            cameraZ = 100;
        
        var engine = new win.BABYLON.Engine(elCanvas, true),
            lightPos = new win.BABYLON.Vector3(cameraX, cameraY, cameraZ),
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

            var matWall = new win.BABYLON.PBRMetallicRoughnessMaterial('matWall', scene);
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
            redMaterial.diffuseColor = new win.BABYLON.Color3(1, 0, 0);
            redMaterial.specularColor = new win.BABYLON.Color3(1, 0.8, 0.8);
            redMaterial.emissiveColor = new win.BABYLON.Color3(1, 0, 0);
            redMaterial.ambientColor = new win.BABYLON.Color3(1, 0, 0);
            redMaterial.alpha = 0.8;

            var blueMaterial = new win.BABYLON.StandardMaterial("blueMaterial", scene);
            blueMaterial.diffuseColor = new win.BABYLON.Color3(0, 0, 1);
            blueMaterial.specularColor = new win.BABYLON.Color3(1, 1, 1);
            blueMaterial.emissiveColor = new win.BABYLON.Color3(0, 0, 1);
            blueMaterial.ambientColor = new win.BABYLON.Color3(0, 0, 1);
            blueMaterial.alpha = 0.8;
            
            var greenMaterial = new win.BABYLON.StandardMaterial("greenMaterial", scene);
            greenMaterial.diffuseColor = new win.BABYLON.Color3(0, 1, 0);
            greenMaterial.specularColor = new win.BABYLON.Color3(1, 1, 1);
            greenMaterial.emissiveColor = new win.BABYLON.Color3(0, 1, 0);
            greenMaterial.ambientColor = new win.BABYLON.Color3(0, 1, 0);
            greenMaterial.alpha = 0.8;

            var yellowMaterial = new win.BABYLON.StandardMaterial("yellowMaterial", scene);
            yellowMaterial.diffuseColor = new win.BABYLON.Color3(0, 0, 0);
            yellowMaterial.specularColor = new win.BABYLON.Color3(1, 1, 1);
            yellowMaterial.emissiveColor = new win.BABYLON.Color3(1, 1, 0.35);
            yellowMaterial.ambientColor = new win.BABYLON.Color3(0, 0, 0);
            yellowMaterial.alpha = 0.8;

            var alphaMaterial = new win.BABYLON.StandardMaterial("alphaMaterial", scene);
            alphaMaterial.alpha = 0;

            // Our invisible torus acts as the center point for our scene
            var torus = win.BABYLON.MeshBuilder.CreateBox('torus', { width: 10, height: 10, depth: 10 }, scene);
            torus.position.y = 50;
            torus.position.x = 0;
            torus.position.z = 0;
            torus.sideOrientation = win.BABYLON.Mesh.FRONTSIDE;
            torus.material = matWall;

            // A fake bulb
            var bulb = win.BABYLON.MeshBuilder.CreateSphere('bulb', { diameter: 4 }, scene);
            bulb.position = lightPos;
            bulb.material = matEmit;

            // Coords 
            var matrix = [
                new win.BABYLON.Vector2(0, 4),
                new win.BABYLON.Vector2(0, -4),
                new win.BABYLON.Vector2(1, -3),
                new win.BABYLON.Vector2(1, 3)
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
           var pMesh = win.BABYLON.Mesh.MergeMeshes([yellowBox, greenBox, blueBox, redBox],
                true,
                true,
                undefined,
                false,
                true);
            pMesh.rotation.y = 0;
            pMesh.rotation.x = Math.PI / 4;
            pMesh.position = torus.position;
            pMesh.position.y = torus.position.y - 50;
            pMesh.isVisible = false; // Hide our original mesh as we'll use the instances we create below

            // Lights

            var light = new win.BABYLON.PointLight('light1', lightPos, scene);
            light.diffuse = lightDiffuse;
            light.intensity = 200;
            light.excludedMeshes.push(bulb);
            pMesh.receiveShadows = true;
            
            // Camera

            var camera = new win.BABYLON.ArcRotateCamera('camera', cameraX, cameraY, cameraZ, torus, scene, true);

            // Action

            var instances = [],
                i = 0, // Index
                p = 0, // Position
                r = 0, // Rotation
                c = 0; // Camera

            // Randomly position our instances
            for (i = 100; i > 0; i--) {
                p += 1;
                var newInstance = pMesh.createInstance("i" + i);
                newInstance.position.x = Math.cos(p) * getRnd(-100);
                newInstance.position.y = Math.cos(p) * getRnd(-100);;
                newInstance.position.z = Math.cos(p) * getRnd(-100);
                instances.push(newInstance);
            }

            // Continually rotate all instances
            engine.runRenderLoop(() => {
                for (i = 0; i < instances.length; i++) {
                    instances[i].rotation.y = i + r;
                }
                r += 0.01;
            });
            
            // As we scroll update our camera position
            $(window).scroll(function() {
                lightPos.x = Math.cos(c) * 100;
                lightPos.z = Math.sin(c) * cameraZ;             
                camera.position = lightPos;
                c += 0.009;
            });
            
            return scene;
        })();
        
        // Render Loop
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