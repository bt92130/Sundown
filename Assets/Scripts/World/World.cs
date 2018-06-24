using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles creation of world (spawn critters, interactables, etc)
//NOTE: EACH NODE NEEDS A MESHFILTER AND MESHRENDERER AS WELL AS MATERIAL ADDED

public class World : MonoBehaviour
{
	public List<GameObject> buildingPrefabs;
	public List<GameObject> wildlifePrefabs;

	public static readonly int RESERVED = 2;
	public static readonly int WORLD_SIZE = GameState.DAYS_TO_WIN;
	public static readonly float NODE_SPACING = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
	public static List<GameObject> nodes {get; private set;}
	public static GameObject activeBuilding {get; private set;}
	private GameObject startNode;
	private Transform trans;
	private MapGenerator mapGen;
	private MeshGenerator meshGen;
	private ColliderGenerator collGen;

	public void Awake()
	{
		startNode = GameObject.Find("Node0");
		activeBuilding = GameObject.Find("ActiveBuilding");
		trans = GetComponent<Transform>();
		mapGen = GetComponent<MapGenerator>();
		meshGen = GetComponent<MeshGenerator>();
		collGen = GetComponent<ColliderGenerator>();
		nodes = new List<GameObject>();
	}
	
	public void DisplayFloor()
	{
		foreach(GameObject node in nodes)
		{
			WorldNode wnode = node.GetComponent<WorldNode>();
			for(int i = 0; i < MapGenerator.ROWS; i++)
			{
				for(int j = 0; j < MapGenerator.COLS; j++)
				{
					if(wnode.map[i, j] == MapGenerator.FLOOR)
					{
						float mapWidth = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
						float mapHeight = MapGenerator.ROWS*MeshGenerator.SQUARE_SIZE;
						float x = node.transform.position.x-mapWidth/2+j*MeshGenerator.SQUARE_SIZE+MeshGenerator.SQUARE_SIZE/2;
						float y = node.transform.position.y+mapHeight/2-i*MeshGenerator.SQUARE_SIZE-MeshGenerator.SQUARE_SIZE/2;
						Debug.DrawLine(new Vector2(x-0.5f, y), new Vector2(x+0.5f, y), Color.cyan, 100f);
					}
				}
			}
		}
	}
	//called by GameState in masterreset
	public void Reset()
	{
		if(nodes.Count == 0)
			generateWorldNodes();
		resetWorldNodes();
		generateMapMeshCollider();
		generateBuildings();
		generateWildlife();
		foreach(GameObject node in nodes)
			node.GetComponent<WorldNode>().ParentReset();
	}

	//populates each worldnode with some wildlife
	private void generateWildlife()
	{
		foreach(GameObject node in nodes)
		{
			WorldNode wnode = node.GetComponent<WorldNode>();
			List<Vector2> points = new List<Vector2>();
			//get list of valid wildlife spawn points
			int count = Random.Range(0, 10);
			for(int i = 0; i < count; i++)
			{
				Vector2 point = getValidPoint(node);
				points.Add(point);	
			}
			//use wildlife object pool or spawn new ones
			for(int i = 0; i < wnode.wildlifePool.Count; i++)
			{
				if(points.Count == 0)
				{
					for(int j = i; j < wnode.wildlifePool.Count; j++)
						wnode.wildlifePool[j].SetActive(false);
					break;
				}
				Vector2 point = points[0];
				GameObject poolObject = wnode.wildlifePool[i];
				Wildlife wildlife = poolObject.GetComponent<Wildlife>();
				poolObject.transform.position = wildlife.SetFloorPosition(point);
				poolObject.SetActive(true);
				wildlife.Reset();
				points.Remove(point);
			}
			for(int i = 0; i < points.Count; i++)
			{
				Vector2 point = points[i];
				GameObject animal = Instantiate(wildlifePrefabs[1], node.transform.Find("Wildlife"));
				Wildlife wildlife = animal.GetComponent<Wildlife>();
				wildlife.Init();
				animal.transform.position = wildlife.SetFloorPosition(point);
				wnode.AddPoolObject(animal, wnode.wildlifePool);
			}
		}
	}

	//use reservedMap to determine where buildings and future things are placed
	private void generateBuildings()
	{
		activeBuilding.SetActive(false);
		foreach(GameObject node in nodes)
		{
			WorldNode wnode = node.GetComponent<WorldNode>();
			List<Vector2> points = new List<Vector2>();
			//get list of valid building spawn points
			int count = Random.Range(0, 5);
			for(int i = 0; i < count; i++)
			{
				Vector2 point = getValidPoint(node);
				points.Add(point);	
			}
			//use building object pool or spawn new ones
			for(int i = 0; i < wnode.buildingPool.Count; i++)
			{
				if(points.Count == 0)
				{
					for(int j = i; j < wnode.buildingPool.Count; j++)
						wnode.buildingPool[j].SetActive(false);
					break;
				}
				Vector2 point = points[0];
				GameObject poolObject = wnode.buildingPool[i];
				Building building = poolObject.GetComponent<Building>();
				poolObject.transform.position = building.SetFloorPosition(point);
				building.Reset();
				poolObject.SetActive(true);
				points.Remove(point);
			}
			for(int i = 0; i < points.Count; i++)
			{
				Vector2 point = points[i];
				GameObject obj = Instantiate(buildingPrefabs[0], node.transform.Find("Buildings"));
				Building building = obj.GetComponent<Building>();
				building.Init();
				obj.transform.position = building.SetFloorPosition(point);
				wnode.AddPoolObject(obj, wnode.buildingPool);
			}
		}
	}
	
	//rolls a new map, mesh, and pool of colliders for walls
	private void generateMapMeshCollider()
	{
		foreach(GameObject node in nodes)
		{
			WorldNode wnode = node.GetComponent<WorldNode>();
			int[,] map = mapGen.GenerateMap();
			wnode.SetMap(map);
			Mesh mesh = meshGen.GenerateMesh(map);
			wnode.meshFilter.mesh = mesh;
			collGen.GenerateCollider(node);
		}
	}

	//deactivates all world nodes
	private void resetWorldNodes()
	{
		foreach(GameObject node in nodes)
			node.SetActive(false);
	}

	//should only be used once at the start to instantiate the nodes
	private void generateWorldNodes()
	{
		if(nodes == null)
		{
			Debug.Log("Error: nodes is null");
			return;
		}
		nodes.Clear();
		startNode.GetComponent<WorldNode>().SetNodeID(nodes.Count);
		nodes.Add(startNode);
		for(int i = 1; i < WORLD_SIZE; i++)
		{
			GameObject node = Instantiate(startNode, trans);
			node.name = "Node" + i;
			node.GetComponent<WorldNode>().SetNodeID(i);
			nodes.Add(node);
			node.GetComponent<Transform>().position = new Vector2(i*NODE_SPACING, 0f);
		}
	}

	//returns a point in the node that is not a wall
	private Vector2 getValidPoint(GameObject node)
	{
		WorldNode wnode = node.GetComponent<WorldNode>();
		while(true)
		{
			float mapWidth = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
			float mapHeight = MapGenerator.ROWS*MeshGenerator.SQUARE_SIZE;
			int row = Random.Range(1, MapGenerator.COLS-2);
			int col = Random.Range(1, MapGenerator.ROWS-2);
			if(wnode.map[row, col] == MapGenerator.FLOOR)
			{
				reserveMapPoint(node, row, col);
				//row,col convert to x,y has to be treating row,col as x,y
				float x = node.transform.position.x-mapWidth/2+col*MeshGenerator.SQUARE_SIZE+MeshGenerator.SQUARE_SIZE/2;
				float y = node.transform.position.y+mapHeight/2-row*MeshGenerator.SQUARE_SIZE-MeshGenerator.SQUARE_SIZE/2;
				Debug.DrawLine(new Vector2(x-2f, y), new Vector2(x+2f, y), Color.cyan, 100f);
				return new Vector2(x, y); 
			}
		}
	}

	private void reserveMapPoint(GameObject node, int row, int col)
	{
		WorldNode wnode = node.GetComponent<WorldNode>();
		wnode.map[row, col] = RESERVED;
	}
}
