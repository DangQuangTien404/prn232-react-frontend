import React from 'react';
import useSlideStore from '@/stores/useSlideStore';

const PropertiesPanel: React.FC = () => {
  const { selectedElement, updateElementStyle } = useSlideStore();

  if (!selectedElement) {
    return <div className="w-64 bg-white border-l border-gray-200 p-4">Select an element to edit its properties.</div>;
  }

  return (
    <div className="w-64 bg-white border-l border-gray-200 p-4">
      <h3 className="font-bold mb-4">Properties</h3>
      <div>
        <label>
          Font Size:
          <input
            type="number"
            onChange={(e) => updateElementStyle(selectedElement, 'fontSize', parseInt(e.target.value))}
            className="w-full p-1 border"
          />
        </label>
      </div>
      <div>
        <label>
          Color:
          <input
            type="color"
            onChange={(e) => updateElementStyle(selectedElement, 'color', e.target.value)}
            className="w-full p-1 border"
          />
        </label>
      </div>
    </div>
  );
};

export default PropertiesPanel;