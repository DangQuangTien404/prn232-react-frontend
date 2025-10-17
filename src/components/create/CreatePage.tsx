import React from 'react';
import Toolbar from './Toolbar';
import Sidebar from './Sidebar';
import Canvas from './Canvas';
import PropertiesPanel from './PropertiesPanel';

const CreatePage: React.FC = () => {
  return (
    <div className="flex h-screen bg-gray-100">
      <Sidebar />
      <div className="flex-1 flex flex-col">
        <Toolbar />
        <div className="flex-1 flex">
          <Canvas />
          <PropertiesPanel />
        </div>
      </div>
    </div>
  );
};

export default CreatePage;