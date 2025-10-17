import React from 'react';
import Element from './Element';
import { Slide } from '@/types/slide';

interface SlideProps {
  slide: Slide;
}

const SlideComponent: React.FC<SlideProps> = ({ slide }) => {
  return (
    <>
      {slide.elements.map((element) => (
        <Element key={element.id} element={element} />
      ))}
    </>
  );
};

export default SlideComponent;