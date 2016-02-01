using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelVisualGenerator : MonoBehaviour {

	public float widthCellGrid = 1;
	public float heightCellGrid = 1;
	public int widthGrid = 8;
	public int heightGrid = 4;

	public Vector3 origin = Vector3.zero;

	public ButtonVisualAspect[] listOfButtons;

	public int[,] validListPositions;

	void Start () {

		validListPositions = new int[widthGrid, heightGrid];
		for (int x = 0; x < widthGrid; x++) {
			for (int y = 0; y < heightGrid; y++) {
				validListPositions [x, y] = 1;
			}
		}
	}

	public void DoPlaceButton() {
        Start();
		foreach(ButtonVisualAspect currentButton in listOfButtons) {
			TryPlaceButton (currentButton);
		}
	}

	public void TryPlaceButton(ButtonVisualAspect button) {
		bool donePosition = false;
		int posX, posY;
		while (donePosition == false) {
			// Random a position
            posX = Random.Range(0, widthGrid - 1);
            posY = Random.Range(0, heightGrid - 1);
			switch (button.typeButton) {
			case TypeButtonVisualAspect.Size1x1:
				// Place 1 by 1 button
				if (validListPositions [posX, posY] == 1) {
					validListPositions [posX, posY] = 0;
                    button.transform.position = new Vector3(origin.x + posX * widthCellGrid, origin.y + posY * heightCellGrid, button.transform.position.z);
					donePosition = true;
				}
				break;
			case TypeButtonVisualAspect.Size1x2:
				// Place 1 by 2 button
				// Check if the size of the button fits on the grid and if there is a valid position for it.
				if ((posY + 1) < widthGrid && validListPositions [posX, posY] == 1
					&& validListPositions [posX, posY+1] == 1) {
					validListPositions [posX, posY] = 0;
					validListPositions [posX, posY+1] = 0;
                    button.transform.position = new Vector3(origin.x + posX * widthCellGrid, origin.y + posY * heightCellGrid, button.transform.position.z);
					donePosition = true;
				}
				break;
			case TypeButtonVisualAspect.Size2x1:
				// Check if the size of the button fits on the grid and if there is a valid position for it.
				if ((posX + 1) < heightGrid && validListPositions [posX, posY] == 1 
					&& validListPositions [posX+1, posY] == 1) {
					validListPositions [posX, posY] = 0;
					validListPositions [posX+1, posY] = 0;
                    button.transform.position = new Vector3(origin.x + posX * widthCellGrid, origin.y + posY * heightCellGrid, button.transform.position.z);
					donePosition = true;
				}
				break;
			case TypeButtonVisualAspect.Size2x2:
				// Check if the size of the button fits on the grid and if there is a valid position for it.
				if ((posX + 1) < widthGrid && (posY + 1) < heightGrid 
					&& validListPositions [posX, posY] == 1 
					&& validListPositions [posX+1, posY] == 1 
					&& validListPositions [posX, posY+1] == 1 
					&& validListPositions [posX+1, posY+1] == 1) {
					validListPositions [posX, posY] = 0;
					validListPositions [posX+1, posY] = 0;
					validListPositions [posX, posY+1] = 0;
					validListPositions [posX+1, posY+1] = 0;
                    button.transform.position = new Vector3(origin.x + posX * widthCellGrid, origin.y + posY * heightCellGrid, button.transform.position.z);
					donePosition = true;
				}
				break;
			}
		}
	}
}