export interface SlideElement {
  id: string;
  type: 'text' | 'image' | 'shape' | 'table' | 'graphic';
  x: number;
  y: number;
  width: number;
  height: number;
  content?: string;
  isEditing?: boolean;
  style?: {
    fontSize?: number;
    fontFamily?: string;
    color?: string;
    backgroundColor?: string;
    borderRadius?: number;
    fontWeight?: 'normal' | 'bold';
    fontStyle?: 'normal' | 'italic';
    textDecoration?: 'none' | 'underline';
    textAlign?: 'left' | 'center' | 'right' | 'justify';
  };
}

export interface Slide {
  id: string;
  elements: SlideElement[];
  backgroundColor: string;
}