import React from 'react';
import { Save, Undo, Redo, FileDown } from 'lucide-react';
import useSlideStore from '@/stores/useSlideStore';

const Toolbar: React.FC = () => {
  const { undo, redo } = useSlideStore();

  return (
    <div className="flex items-center justify-between bg-white border-b border-gray-200 px-4 py-2 z-20">
      <div className="flex items-center gap-4">
        <h1 className="text-xl font-bold text-gray-900">Create Slide</h1>
        <div className="flex items-center gap-2">
          <button
            onClick={undo}
            className="p-2 rounded hover:bg-gray-100"
          >
            <Undo size={20} />
          </button>
          <button
            onClick={redo}
            className="p-2 rounded hover:bg-gray-100"
          >
            <Redo size={20} />
          </button>
        </div>
      </div>
      <div className="flex items-center gap-2">
        <button className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
          <FileDown size={16} />
          Export PPTX
        </button>
        <button className="flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700">
          <Save size={16} />
          Save to Cloud
        </button>
      </div>
    </div>
  );
};

export default Toolbar;