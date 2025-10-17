import React from 'react';
import useSlideStore from '@/stores/useSlideStore';
import SlideComponent from './Slide';

const Canvas: React.FC = () => {
  const { slides, currentSlideIndex } = useSlideStore();
  const currentSlide = slides[currentSlideIndex];

  return (
    <div className="flex-1 flex items-center justify-center p-8 bg-gray-200">
      <div
        className="relative bg-white shadow-lg"
        style={{
          width: '800px',
          height: '600px',
          backgroundColor: currentSlide.backgroundColor,
        }}
      >
        <SlideComponent slide={currentSlide} />
      </div>
    </div>
  );
};

export default Canvas;