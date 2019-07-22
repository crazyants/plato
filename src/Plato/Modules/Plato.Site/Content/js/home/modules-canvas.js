$(function (win, doc, $) {


   
    function getRnd(max) {
        return Math.floor(Math.random() * Math.floor(max));
    }

    var elCanvas = doc.getElementById("moduleCanvas");
    var engine = new win.BABYLON.Engine(elCanvas, true);
    var lightPos = new win.BABYLON.Vector3(0, 50, 0);
    var lightDiffuse = new win.BABYLON.Color3(1, 1, 1);


    // scene
    var scene = (() => {
        var scene = new win.BABYLON.Scene(engine);
        scene.clearColor = new win.BABYLON.Color3(0.9, 0.9, 0.9);

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

        var redColor = new win.BABYLON.Color3(1, 0, 0);

        var redMaterial = new win.BABYLON.StandardMaterial("redMaterial", scene);
        redMaterial.diffuseColor = new win.BABYLON.Color3(1, 0, 0);
        redMaterial.specularColor = new win.BABYLON.Color3(1, 0.8, 0.8);
        redMaterial.emissiveColor = redColor;
        redMaterial.ambientColor = new win.BABYLON.Color3(1, 0, 0);
        redMaterial.alpha = 0.8;

        var blueMaterial = new win.BABYLON.StandardMaterial("blueMaterial", scene);
        blueMaterial.diffuseColor = new win.BABYLON.Color3(0, 0, 1);
        blueMaterial.specularColor = new win.BABYLON.Color3(1, 1, 1);
        blueMaterial.emissiveColor = new win.BABYLON.Color3(0, 0, 1);
        blueMaterial.ambientColor = new win.BABYLON.Color3(0, 0, 1);
        blueMaterial.alpha = 0.8;


        var greenMaterial = new BABYLON.StandardMaterial("greenMaterial", scene);
        greenMaterial.diffuseColor = new win.BABYLON.Color3(0, 1, 0);
        greenMaterial.specularColor = new win.BABYLON.Color3(1, 1, 1);
        greenMaterial.emissiveColor = new win.BABYLON.Color3(0, 1, 0);
        greenMaterial.ambientColor = new win.BABYLON.Color3(0, 1, 0);
        greenMaterial.alpha = 0.8;

        var yellowMaterial = new BABYLON.StandardMaterial("yellowMaterial", scene);
        yellowMaterial.diffuseColor = new win.BABYLON.Color3(1, 1, 0.35);
        yellowMaterial.specularColor = new win.BABYLON.Color3(1, 1, 1);
        yellowMaterial.emissiveColor = new win.BABYLON.Color3(1, 1, 0.35);
        yellowMaterial.ambientColor = new win.BABYLON.Color3(0, 0, 0);
        yellowMaterial.alpha = 0.8;

        var alphaMaterial = new win.BABYLON.StandardMaterial("alphaMaterial", scene);
        alphaMaterial.alpha = 0;

        // meshes
        var torus = win.BABYLON.MeshBuilder.CreateBox('torus', { width: 10, height: 10, depth: 10 }, scene);
        torus.position.y = 50;
        torus.position.x = 0;
        torus.position.z = 0;
        torus.sideOrientation = win.BABYLON.Mesh.FRONTSIDE;
        torus.material = alphaMaterial;

        var bulb = win.BABYLON.MeshBuilder.CreateSphere('bulb', { diameter: 4 }, scene);
        bulb.position = lightPos;
        bulb.material = alphaMaterial;

        // ---------------------

        var corners = [
            new win.BABYLON.Vector2(0, 4),
            new win.BABYLON.Vector2(0, -4),
            new win.BABYLON.Vector2(1, -3),
            new win.BABYLON.Vector2(1, 3),
        ];

        var polyMesh = new win.BABYLON.PolygonMeshBuilder("polytri2", corners, scene);

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

        // ------------------
        // Merge Meshes
        // ------------------

        var pMesh = win.BABYLON.Mesh.MergeMeshes([yellowBox, greenBox, blueBox, redBox],
            true,
            true,
            undefined,
            false,
            true);
        pMesh.rotation.y = 0;
        pMesh.rotation.x = Math.PI / 4;
        pMesh.position = torus.position;

        // camera
        var camera = new win.BABYLON.ArcRotateCamera('camera', 0, Math.PI * 3 / 8, 200, torus, scene, true);
        camera.upperBetaLimit = Math.PI * 3 / 4;
        camera.wheelPrecision = 10;
        camera.upperRadiusLimit = 200;
        camera.lowerRadiusLimit = 5;
        camera.attachControl(elCanvas, true);

        var light1;
        {
            var light = new win.BABYLON.PointLight('light1', lightPos, scene);
            light.diffuse = lightDiffuse;
            light.intensity = 0;
            light.excludedMeshes.push(bulb);
            var shadowGen = new win.BABYLON.ShadowGenerator(1024, light);
            shadowGen.bias = 0.0005;
            shadowGen.usePercentageCloserFiltering = true;
            shadowGen.setDarkness(0.3);
            shadowGen.addShadowCaster(pMesh);
            pMesh.receiveShadows = true;
            light1 = light;
        }


        pMesh.isVisible = false;
        var instances = [];
        var i = 0;
        for (var index = 0; index < 100; index++) {

            i += 0.003;
            lightPos.x = Math.cos(i-10) * 50;
            lightPos.z = Math.sin(i) * 50;
            var newInstance = pMesh.createInstance("i" + index);

            newInstance.position.x = lightPos.x * (Math.PI / 10 + Math.sin(index * 4));
            newInstance.position.y = lightPos.y * (Math.PI / 10 + Math.sin(index));
            newInstance.position.z = lightPos.z * (Math.PI / 10 + Math.sin(index * 10));


            instances.push(newInstance);
        }
        
      
        var i = 0;
        engine.runRenderLoop(() => {

            i += 0.006;
          
            lightPos.x = Math.cos(i) * 100;
            lightPos.z = Math.sin(i) * 100;

            for (var index = 0; index < instances.length; index++) {
                
                //instances[index].position.x = -Math.cos(t) * 10
                instances[index].position.x = lightPos.x * (Math.PI / 10 + Math.sin(index * 4));
                instances[index].position.y = lightPos.y * (Math.PI / 10 + Math.sin(index));
                instances[index].position.z = lightPos.z * (Math.PI / 10 + Math.sin(index * 10));

                instances[index].rotation.z = Math.PI;
                instances[index].rotation.y = i;
                instances[index].rotation.x = i;


            }



            camera.position = lightPos;

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


}(window, document, jQuery));