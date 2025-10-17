import React from 'react';
import { Rnd } from 'react-rnd';
import useSlideStore from '@/stores/useSlideStore';
import { Image, Square, Table, Shapes } from 'lucide-react';
import { SlideElement } from '@/types/slide';

interface ElementProps {
  element: SlideElement;
}

const Element: React.FC<ElementProps> = ({ element }) => {
  const { updateElementStyle } = useSlideStore();

  return (
    <Rnd
      size={{ width: element.width, height: element.height }}
      position={{ x: element.x, y: element.y }}
      onDragStop={(e, d) => {
        updateElementStyle(element.id, 'x', d.x);
        updateElementStyle(element.id, 'y', d.y);
      }}
      onResizeStop={(e, direction, ref, delta, position) => {
        updateElementStyle(element.id, 'width', ref.style.width);
        updateElementStyle(element.id, 'height', ref.style.height);
        updateElementStyle(element.id, 'x', position.x);
        updateElementStyle(element.id, 'y', position.y);
      }}
    >
      <div
        className="w-full h-full"
        style={{
          ...element.style,
        }}
      >
        {element.type === 'text' && <div>{element.content}</div>}
        {element.type === 'image' && <Image className="w-full h-full" />}
        {element.type === 'shape' && <Square className="w-full h-full" />}
        {element.type === 'table' && <Table className="w-full h-full" />}
        {element.type === 'graphic' && <Shapes className="w-full h-full" />}
      </div>
    </Rnd>
  );
};

export default Element;