import React, { useState, useRef, useEffect, type ReactElement } from 'react';

import HeaderToolbar from './top';
import Main from './main';


export default function SqlBotChat() {

  return (
    <div className="flex flex-col h-screen w-full bg-gray-50 text-gray-800">
      <HeaderToolbar />

      <Main />
    </div>
  );
}


