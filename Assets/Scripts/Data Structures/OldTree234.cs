﻿using UnityEngine;
using System.Collections;

public class OldTree234 : MonoBehaviour {

	public class DataItem {
		public long dData;
		
		public DataItem(long dd){
			dData = dd;
		}
		
		public void displayItem(){
			System.Console.Write("/" + dData);
		}
	}
	
	public class Node{
		private static readonly int ORDER = 4;
		private int numItems;
		private Node parent;
		private Node[] childArray = new Node[ORDER];
		private DataItem[] itemArray = new DataItem[ORDER - 1];
		
		// connect child to this node
		public void connectChild(int childNum, Node child){
			childArray [childNum] = child;
			if (child != null)
				child.parent = this;
		}
		// disconnect child from this node, return it
		public Node disconnectChild(int childNum){
			Node tempNode = childArray [childNum];
			childArray [childNum] = null;
			return tempNode;
		}
		
		public Node getChild(int childNum) {
			return childArray [childNum];
		}
		
		public Node getParent(){
			return parent;
		}
		
		public bool isLeaf(){
			return (childArray [0] == null) ? true : false;
		}
		
		public int getNumItems(){
			return numItems;
		}
		
		public DataItem getItem(int index){
			return itemArray [index];
		}
		
		public bool isFull(){
			return (numItems == ORDER - 1) ? true : false;
		}
		
		public int findItem(long key){ // return index of item (within node)
			
			for (int j = 0; j < ORDER - 1; j++){  
				if(itemArray[j] == null) 
					break;
				else if (itemArray[j].dData == key)
					return j;
			}
			return -1;
		}
		
		public int insertItem(DataItem newItem){
			//assumes node is not full
			numItems++;
			long newKey = newItem.dData;
			
			for (int j = ORDER - 2; j >= 0; j--) {
				if (itemArray[j] == null)
					continue;
				else{
					long itsKey = itemArray[j].dData;
					if (newKey < itsKey)
						itemArray[j + 1] = itemArray[j];
					else{
						itemArray[j + 1] = newItem;
						return j + 1;
					}
				}
			}
			itemArray [0] = newItem;
			return 0;
		}
		
		public DataItem removeItem(){ // remove largest item
			//assumes node is not empty
			DataItem temp = itemArray [numItems - 1];
			itemArray [numItems - 1] = null;
			numItems--;
			return temp;
		}
		
		public void displayNode(){
			for (int j = 0; j < numItems; j++)
				itemArray [j].displayItem ();
			System.Console.Write ("/"); 
		}
		
	}
	
	public class tree234 {
		private Node root = new Node (); // make root node
		
		public int find(long key) {
			Node curNode = root;
			int childNumber;
			while (true) {
				if((childNumber = curNode.findItem(key)) != -1)
					return childNumber; //found it
				else if(curNode.isLeaf())
					return -1; // can't find it
				else {
					curNode = getNextChild(curNode, key); //search deeper
				}
			}
		}
		
		// insert a DataItem
		public void insert(long dValue) {
			Node curNode = root;
			DataItem tempItem = new DataItem(dValue);
			while (true) {
				if (curNode.isFull()) // if node full,
				{
					split(curNode); // split it
					curNode = curNode.getParent(); // back up
					// search once
					curNode = getNextChild(curNode, dValue);
				} // end if(node is full)
				else if (curNode.isLeaf()) // if node is leaf,
					break; // go insert
				// node is not full, not a leaf; so go to lower level
				else
					curNode = getNextChild(curNode, dValue);
			} // end while
			curNode.insertItem(tempItem); // insert new DataItem
		}
		
		// split the node
		public void split(Node thisNode) // split the node
		{
			// assumes node is full
			DataItem itemB, itemC;
			Node parent, child2, child3;
			int itemIndex;
			itemC = thisNode.removeItem(); // remove items from
			itemB = thisNode.removeItem(); // this node
			child2 = thisNode.disconnectChild(2); // remove children
			child3 = thisNode.disconnectChild(3); // from this node
			Node newRight = new Node(); // make new node
			if (thisNode == root) // if this is the root,
			{
				root = new Node(); // make new root
				parent = root; // root is our parent
				root.connectChild(0, thisNode); // connect to parent
			} else
				// this node not the root
				parent = thisNode.getParent(); // get parent
			// deal with parent
			itemIndex = parent.insertItem(itemB); // item B to parent
			int n = parent.getNumItems(); // total items?
			for (int j = n - 1; j > itemIndex; j--) // move parent's
			{ // connections
				Node temp = parent.disconnectChild(j); // one child
				parent.connectChild(j + 1, temp); // to the right
			}
			// connect newRight to parent
			parent.connectChild(itemIndex + 1, newRight);
			// deal with newRight
			newRight.insertItem(itemC); // item C to newRight
			newRight.connectChild(0, child2); // connect to 0 and 1
			newRight.connectChild(1, child3); // on newRight
		} 
		
		// gets appropriate child of node during search for value
		public Node getNextChild(Node theNode, long theValue) {
			int j;
			// assumes node is not empty, not full, not a leaf
			int numItems = theNode.getNumItems();
			for (j = 0; j < numItems; j++) // for each item in node
			{ // are we less?
				if (theValue < theNode.getItem(j).dData)
					return theNode.getChild(j); // return left child
			} // end for // we're greater, so
			return theNode.getChild(j); // return right child
		}
		
		public void displayTree() {
			recDisplayTree(root, 0, 0);
		}
		
		private void recDisplayTree(Node thisNode, int level, int childNumber) {
			System.Console.Write("level= " + level + " child= " + childNumber + " ");
			thisNode.displayNode(); // display this node
			// call ourselves for each child of this node
			int numItems = thisNode.getNumItems();
			for (int j = 0; j < numItems + 1; j++) {
				Node nextNode = thisNode.getChild(j);
				if (nextNode != null)
					recDisplayTree(nextNode, level + 1, j);
				else
					return;
			}
		} // end recDisplayTree()
	}
}
